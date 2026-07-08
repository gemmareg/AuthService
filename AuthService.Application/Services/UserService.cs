using AuthService.Application.Abstractions.Events;
using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Application.Dtos;
using Auth.Contracts.Events;
using AuthService.Domain;
using AuthService.Domain.ValueObjects;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Constants;
using AuthService.Shared.Result.NonGeneric;
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

            var accessToken = await tokenService.GenerateAccessTokenAsync(user);

            var refreshTokenResult = await tokenService.GenerateRefreshToken(user.Id);
            if (!refreshTokenResult.Success)
            {
                logger.LogError("Refresh token generation failed for user ID: {UserId}. Reason: {Reason}", user.Id, refreshTokenResult.Message);
                return Result<AuthResponse>.Fail(refreshTokenResult.Message);
            }
            var refreshToken = refreshTokenResult.Data!;

            logger.LogInformation("User registered successfully with email: {Email}", email);

            await PublishRegisteredEventAsync(user);

            return Result<AuthResponse>.Ok(
                new AuthResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.Expiration
                }
            );
        }

        public async Task<Result> SoftDeleteAsync(Guid userId, Guid requesterId)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                logger.LogWarning("Soft delete attempted for non-existent user ID: {UserId}", userId);
                return Result.Fail("User not found");
            }

            var isSelf = requesterId == userId;

            // La autorización (self, o el permiso "users:delete:any" según el
            // claim del token) ya se comprueba en el controller — no se repite
            // aquí contra la BD; el servicio confía en que quien lo llama ya
            // validó el acceso.
            var softDeleteResult = user.SoftDelete();
            if (!softDeleteResult.Success)
            {
                logger.LogWarning("Soft delete failed for user ID: {UserId}. Reason: {Reason}", userId, softDeleteResult.Message);
                return Result.Fail(softDeleteResult.Message);
            }

            user.SetUpdated(isSelf ? $"Self:{requesterId}" : $"ByOther:{requesterId}");

            await userRepository.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("User soft deleted successfully with ID: {UserId}", userId);

            await PublishSoftDeletedEventAsync(user);

            return Result.Ok();
        }

        public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
        {
            var user = await userRepository.GetByEmailWithRolesAsync(email);

            if (user is null || !passwordService.Verify(password, user.PasswordHash))
            {
                logger.LogWarning("Failed login attempt for email: {Email}", email);
                return Result<AuthResponse>.Fail("Invalid email or password");
            }

            if (!user.IsActive)
            {
                if (user.UpdatedBy?.StartsWith("ByOther:", StringComparison.OrdinalIgnoreCase) == true)
                {
                    logger.LogWarning("Login forbidden for account deactivated by another user with email: {Email}", email);
                    return Result<AuthResponse>.Fail(UserErrorMessages.AccountDeactivatedByAdmin);
                }

                user.Activate();
                await unitOfWork.SaveChangesAsync();

                await PublishActivatedEventAsync(user);
            }

            var accessToken = await tokenService.GenerateAccessTokenAsync(user);

            var refreshTokenResult = await tokenService.GenerateRefreshToken(user.Id);
            if (!refreshTokenResult.Success)
            {
                logger.LogError("Refresh token generation failed for user ID: {UserId}. Reason: {Reason}", user.Id, refreshTokenResult.Message);
                return Result<AuthResponse>.Fail(refreshTokenResult.Message);
            }

            await unitOfWork.SaveChangesAsync();

            var refreshToken = refreshTokenResult.Data!;

            logger.LogInformation("User logged in successfully with email: {Email}", email);

            return Result<AuthResponse>.Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.Expiration
            });
        }

        public async Task<Result> UpdateAsync(Guid userId, string name, string surname, string email)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("Update attempted for non-existent user ID: {UserId}", userId);
                return Result.Fail("User not found");
            }
            if (user.Email != email)
            {
                var existingUserWithEmail = await userRepository.GetByEmailAsync(email);
                if (existingUserWithEmail is not null && existingUserWithEmail.Id != userId)
                {
                    logger.LogWarning("Update failed due to email conflict. User ID: {UserId}, Email: {Email}", userId, email);
                    return Result.Fail("Email already in use by another account");
                }
                user.UpdateEmail(email);
                await PublishEmailUpdatedEventAsync(user);
            }

            user.UpdateName(name, surname);
            await userRepository.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("User updated successfully with ID: {UserId}", userId);
            return Result.Ok();
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
                    .Select(u =>
                    {
                        var numberPart = u.Substring(username.Length);
                        return int.TryParse(numberPart, out var n) ? n : 0;
                    })
                    .Max();

            return username + (nextNumber + 1);
        }

        private Task PublishRegisteredEventAsync(User user)
        {
            var evt = new UserRegisteredEvent(user.Id.ToString(), user.Name);

            return publisher.PublishUserRegisteredAsync(evt);
        }

        private Task PublishSoftDeletedEventAsync(User user)
        {
            UserSoftDeletedEvent evt = new(user.Id.ToString());

            return publisher.PublishUserSoftDeletedAsync(evt);
        }

        private Task PublishActivatedEventAsync(User user)
        {
            UserActivatedEvent evt = new(user.Id.ToString());
            return publisher.PublishUserActivatedAsync(evt);
        }

        private Task PublishEmailUpdatedEventAsync(User user)
        {
            EmailChangedEvent evt = new(user.Id.ToString(), user.Email);
            return publisher.PublishEmailChangedAsync(evt);
        }
    }
}
