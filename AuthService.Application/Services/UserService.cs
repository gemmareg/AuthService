using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Domain;
using AuthService.Domain.ValueObjects;
using AuthService.Shared.Result.Generic;

namespace AuthService.Application.Services
{
    public class UserService(
        IPasswordService passwordService,
        ITokenService tokenService,
        IUserRepository userRepository,
        IRoleRepository roleRepository) : IUserService
    {
        public async Task<Result<AuthResponse>> RegisterAsync(string name, string surname, string email, string password)
        {
            var existingUser = await userRepository.GetByEmailAsync(email);
            var role = await roleRepository.GetByNameAsync("User");

            if (existingUser is not null)
                return Result<AuthResponse>.Fail("Email already registered");

            var passwordHash = passwordService.Hash(password);
            var username = await GenerateUsername(name, surname);

            var userResult = User.Create(
                username,
                email,
                passwordHash,
                name,
                surname);

            if (!userResult.Success) return Result<AuthResponse>.Fail(userResult.Message);

            var user = userResult.Data!;
            user.AssignRole(role);

            await userRepository.AddAsync(user);

            var accessToken = tokenService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.Roles.Select(r => r.Name));

            var refreshTokenResult = await tokenService.GenerateRefreshToken(user.Id);
            if(!refreshTokenResult.Success) return Result<AuthResponse>.Fail(refreshTokenResult.Message);
            var refreshToken = refreshTokenResult.Data!;

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
    }
}
