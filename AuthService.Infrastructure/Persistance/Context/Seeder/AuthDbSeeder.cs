using AuthService.Application.Abstractions.Events;
using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Domain;
using AuthService.Infrastructure.Extensions.Options;
using Auth.Contracts.Events;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Persistance.Context.Seeder
{
    public class AuthDbSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;
        private readonly AdminSeedSettings _adminSeed;

        public AuthDbSeeder(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordService passwordService,
            IUnitOfWork unitOfWork,
            IEventPublisher eventPublisher,
            IOptions<AdminSeedSettings> adminSeedOptions)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
            _adminSeed = adminSeedOptions.Value;
        }

        public async Task SeedAsync()
        {
            if (string.IsNullOrWhiteSpace(_adminSeed.UserId) ||
                string.IsNullOrWhiteSpace(_adminSeed.Email) ||
                string.IsNullOrWhiteSpace(_adminSeed.Password) ||
                string.IsNullOrWhiteSpace(_adminSeed.Username))
            {
                throw new InvalidOperationException("AdminSeed configuration is invalid. UserId, Username, Email and Password are required.");
            }

            if (!Guid.TryParse(_adminSeed.UserId, out var adminUserId))
            {
                throw new InvalidOperationException("AdminSeed configuration is invalid. UserId must be a valid GUID.");
            }

            var adminRole = await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("User");

            var existingAdmin = await _userRepository.GetByEmailAsync(_adminSeed.Email);
            if (existingAdmin == null)
            {
                var passwordHash = _passwordService.Hash(_adminSeed.Password);
                var userResult = User.Create(
                    _adminSeed.Username,
                    _adminSeed.Email,
                    passwordHash,
                    _adminSeed.Name,
                    _adminSeed.Surname,
                    adminUserId
                );

                if (!userResult.Success)
                    throw new Exception("No se pudo crear el usuario administrador: " + userResult.Message);

                var adminUser = userResult.Data!;
                adminUser.AssignRole(adminRole);
                await _userRepository.AddAsync(adminUser);

                await _unitOfWork.SaveChangesAsync();

                _eventPublisher.PublishAdminCreated(
                    new AdminCreatedEvent(adminUser.Id.ToString(), adminUser.Email, adminUser.Username));

                return;
            }

            if (existingAdmin.Id != adminUserId)
            {
                throw new InvalidOperationException($"Configured AdminSeed UserId ({adminUserId}) does not match existing admin Id ({existingAdmin.Id}).");
            }

            await _unitOfWork.SaveChangesAsync();

            _eventPublisher.PublishAdminCreated(
                new AdminCreatedEvent(existingAdmin.Id.ToString(), existingAdmin.Email, existingAdmin.Username));
        }

        private async Task<Role> EnsureRoleAsync(string roleName)
        {
            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role != null) return role;

            var newRole = Role.Create(roleName).Data!;
            await _roleRepository.AddAsync(newRole);
            return newRole;
        }
    }
}
