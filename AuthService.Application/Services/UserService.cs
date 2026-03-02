using AuthService.Application.Abstractions.Events;
using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Application.Dtos;
using AuthService.Application.Dtos.Events;
using AuthService.Domain;
using AuthService.Domain.ValueObjects;
using AuthService.Shared.Result.Generic;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Services
{
    public class UserService(
        IPasswordService passwordService,
        ITokenGenerator tokenService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IEventPublisher publisher,
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger) : IUserService
    {
        public async Task<Result<AuthResponse>> RegisterAsync(string name, string surname, string email, string password)
        {
            var existingUser = await userRepository.GetByEmailAsync(email);
            var role = await roleRepository.GetByNameAsync("User");

            if (existingUser is not null)
            {
                logger.LogWarning("Registration attempt with already registered email: {Email}", email);
                return Result<AuthResponse>.Fail("Email already registered");
            }

            var passwordHash = passwordService.Hash(password);
            var username = await GenerateUsername(name, surname);

            var userResult = User.Create(
                username,
                email,
                passwordHash,
                name,
                surname);

            if (!userResult.Success)
            {
                logger.LogError("User creation failed for email: {Email}. Reason: {Reason}", email, userResult.Message);
                return Result<AuthResponse>.Fail(userResult.Message);
            }

            var user = userResult.Data!;
            user.AssignRole(role);

            await userRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            var accessToken = tokenService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.Roles.Select(r => r.Name));

            var refreshTokenResult = await tokenService.GenerateRefreshToken(user.Id);
            if (!refreshTokenResult.Success)
            {
                logger.LogError("Refresh token generation failed for user ID: {UserId}. Reason: {Reason}", user.Id, refreshTokenResult.Message);
                return Result<AuthResponse>.Fail(refreshTokenResult.Message);
            }
            var refreshToken = refreshTokenResult.Data!;

            logger.LogInformation("User registered successfully with email: {Email}", email);

            publishEvent(user);

            return Result<AuthResponse>.Ok(
                new AuthResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.Expiration
                }
            );
        }

        private async Task<string> GenerateUsername(string name, string surname)
        {
            var username = (name + surname).ToLower().Replace(" ", "");
            var existingUsernames = await userRepository.GetByUsernameAsync(username);
            if ((existingUsernames.Any(u => u.Username == username))) return username + "1";

            var validUsernames = existingUsernames
                .Where(u => Username.IsValid(u.Username, username))
                .Select(u => u.Username);

            if (!validUsernames.Any()) return username;

            int nextNumber = 1;
            nextNumber = validUsernames
                    .Select(u => {
                        var numberPart = u.Substring(username.Length);
                        return int.TryParse(numberPart, out var n) ? n : 0;
                    })
                    .Max();

            return username + (nextNumber + 1);
        }

        private void publishEvent(User user)
        {
            var evt = new UserRegisteredEvent
            {
                UserId = user.Id.ToString(),
                Name = user.Name,
            };

            publisher.PublishUserRegistered(evt);
        }
    }
}
