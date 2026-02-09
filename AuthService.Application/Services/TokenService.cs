using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Application.Extensions.Options;
using AuthService.Domain;
using AuthService.Domain.Policies;
using AuthService.Shared.Result.Generic;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static AuthService.Shared.Enums;

namespace AuthService.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository, IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
            _tokenRepository = tokenRepository;
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result<RefreshToken>> GenerateRefreshToken(Guid userId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var tokenValue = Convert.ToBase64String(randomBytes);
            var hash = HashToken(tokenValue);

            var tokenResult = Token.Create(userId, TokenType.Refresh, hash);

            if (!tokenResult.Success) return Result<RefreshToken>.Fail("Token creation failed");

            var domainToken = tokenResult.Data!;
            await _tokenRepository.AddAsync(domainToken);

            return Result<RefreshToken>.Ok(new() { Token = tokenValue, Expiration = domainToken.ExpiresAt });
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
