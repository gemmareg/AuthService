using AuthService.Application.Abstractions.Events;
using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Domain;
using AuthService.Infrastructure.Extensions.Options;
using Auth.Contracts;
using Auth.Contracts.Events;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Persistance.Context.Seeder
{
    public class AuthDbSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;
        private readonly AdminSeedSettings _adminSeed;
        private readonly EventPublisherSeedSettings _eventPublisherSeed;

        public AuthDbSeeder(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IPasswordService passwordService,
            IUnitOfWork unitOfWork,
            IEventPublisher eventPublisher,
            IOptions<AdminSeedSettings> adminSeedOptions,
            IOptions<EventPublisherSeedSettings> eventPublisherSeedOptions)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
            _adminSeed = adminSeedOptions.Value;
            _eventPublisherSeed = eventPublisherSeedOptions.Value;
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

            // Debe sembrarse antes que el Admin: en cuanto exista el Admin, se
            // publica AdminCreated por HTTP, lo que exige que la cuenta de
            // servicio EventPublisher ya pueda autenticarse.
            await SeedEventPublisherAsync();

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

                await _eventPublisher.PublishAdminCreatedAsync(
                    new AdminCreatedEvent(adminUser.Id.ToString(), adminUser.Email, adminUser.Username));

                return;
            }

            if (existingAdmin.Id != adminUserId)
            {
                throw new InvalidOperationException($"Configured AdminSeed UserId ({adminUserId}) does not match existing admin Id ({existingAdmin.Id}).");
            }

            await _unitOfWork.SaveChangesAsync();

            await _eventPublisher.PublishAdminCreatedAsync(
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

        private async Task SeedEventPublisherAsync()
        {
            if (string.IsNullOrWhiteSpace(_eventPublisherSeed.UserId) ||
                string.IsNullOrWhiteSpace(_eventPublisherSeed.Email) ||
                string.IsNullOrWhiteSpace(_eventPublisherSeed.Password) ||
                string.IsNullOrWhiteSpace(_eventPublisherSeed.Username))
            {
                throw new InvalidOperationException("EventPublisherSeed configuration is invalid. UserId, Username, Email and Password are required.");
            }

            if (!Guid.TryParse(_eventPublisherSeed.UserId, out var eventPublisherUserId))
            {
                throw new InvalidOperationException("EventPublisherSeed configuration is invalid. UserId must be a valid GUID.");
            }

            var permission = await _permissionRepository.GetByNameAsync(AuthPermissions.EventsPublish);
            if (permission is null)
            {
                permission = Permission.Create(AuthPermissions.EventsPublish, "Publish events to MessageBrokerService.").Data!;
                await _permissionRepository.AddAsync(permission);
            }

            var role = await _roleRepository.GetByNameAsync("EventPublisher");
            if (role is null)
            {
                role = Role.Create("EventPublisher").Data!;
                await _roleRepository.AddAsync(role);
            }

            if (!role.Permissions.Any(p => p.Id == permission.Id || p.Name == permission.Name))
            {
                role.AddPermission(permission);
            }

            var existing = await _userRepository.GetByEmailAsync(_eventPublisherSeed.Email);
            if (existing == null)
            {
                var passwordHash = _passwordService.Hash(_eventPublisherSeed.Password);
                var userResult = User.Create(
                    _eventPublisherSeed.Username,
                    _eventPublisherSeed.Email,
                    passwordHash,
                    _eventPublisherSeed.Name,
                    _eventPublisherSeed.Surname,
                    eventPublisherUserId
                );

                if (!userResult.Success)
                    throw new Exception("No se pudo crear la cuenta de servicio EventPublisher: " + userResult.Message);

                var eventPublisherUser = userResult.Data!;
                eventPublisherUser.AssignRole(role);
                await _userRepository.AddAsync(eventPublisherUser);

                await _unitOfWork.SaveChangesAsync();

                await _eventPublisher.PublishUserRegisteredAsync(
                    new UserRegisteredEvent(eventPublisherUser.Id.ToString(), eventPublisherUser.Name));

                return;
            }

            if (existing.Id != eventPublisherUserId)
            {
                throw new InvalidOperationException($"Configured EventPublisherSeed UserId ({eventPublisherUserId}) does not match existing user Id ({existing.Id}).");
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
