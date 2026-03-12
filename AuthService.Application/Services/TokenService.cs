using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Application.Dtos;
using AuthService.Application.Extensions.Options;
using AuthService.Domain;
using AuthService.Domain.Policies;
using AuthService.Shared.Result.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static AuthService.Shared.Enums;

namespace AuthService.Application.Services
{
    public class TokenService : ITokenGenerator, ITokenRefresher
    {
        private readonly JwtSettings _settings;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TokenService> _logger;

        public TokenService(ITokenRepository tokenRepository, IOptions<JwtSettings> settings, IUnitOfWork unitOfWork, ILogger<TokenService> logger)
        {
            _settings = settings.Value;
            _tokenRepository = tokenRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var expiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Access));

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            _logger.LogInformation("Generated access token for user {UserId} with email {Email}", userId, email);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result<RefreshToken>> GenerateRefreshToken(Guid userId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var tokenValue = Convert.ToBase64String(randomBytes);
            var hash = HashToken(tokenValue);

            var tokenResult = Token.Create(userId, TokenType.Refresh, hash);

            if (!tokenResult.Success) return Result<RefreshToken>.Fail(tokenResult.Message);

            var domainToken = tokenResult.Data!;
            await _tokenRepository.AddAsync(domainToken);

            _logger.LogInformation("Generated refresh token for user {UserId}", userId);

            return Result<RefreshToken>.Ok(new() { Token = tokenValue, Expiration = domainToken.ExpiresAt });
        }
        
        public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var token = await _tokenRepository.GetByTokenHashAsync(tokenHash);

            if (token is null || token.IsExpired || token.IsRevoked)
            {
                _logger.LogWarning("Invalid refresh token attempt");
                return Result<AuthResponse>.Fail("Invalid refresh token");
            }

            var user = token.User;

            if (!user.IsActive)
            {
                _logger.LogWarning("Attempt to refresh token for inactive user {UserId}", user.Id);
                return Result<AuthResponse>.Fail("User account is inactive");
            }

            var newAccessToken = GenerateAccessToken(
            user.Id,
            user.Email,
            user.Roles.Select(r => r.Name));

            var newRefreshToken = await GenerateRefreshToken(user.Id);
            token.Revoke();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Refreshed tokens for user {UserId}", user.Id);

            return Result<AuthResponse>.Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Data!.Token,
                RefreshTokenExpiration = newRefreshToken.Data.Expiration
            });
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
