using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Domain;
using AuthService.Infrastructure.Extensions.Options;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Persistance.Context.Seeder
{
    public class AuthDbSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AdminSeedSettings _adminSeed;

        public AuthDbSeeder(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordService passwordService,
            IUnitOfWork unitOfWork,
            IOptions<AdminSeedSettings> adminSeedOptions)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
            _adminSeed = adminSeedOptions.Value;
        }

        public async Task SeedAsync()
        {
            if (string.IsNullOrWhiteSpace(_adminSeed.Email) ||
                string.IsNullOrWhiteSpace(_adminSeed.Password) ||
                string.IsNullOrWhiteSpace(_adminSeed.Username))
            {
                throw new InvalidOperationException("AdminSeed configuration is invalid. Username, Email and Password are required.");
            }

            // 1. Roles predefinidos
            var adminRole = await EnsureRoleAsync("Admin");
            var userRole = await EnsureRoleAsync("User");

            // 2. Usuario administrador inicial
            var existingAdmin = await _userRepository.GetByEmailAsync(_adminSeed.Email);
            if (existingAdmin == null)
            {
                var passwordHash = _passwordService.Hash(_adminSeed.Password);
                var userResult = User.Create(
                    _adminSeed.Username,
                    _adminSeed.Email,
                    passwordHash,
                    _adminSeed.Name,
                    _adminSeed.Surname
                );

                if (!userResult.Success)
                    throw new Exception("No se pudo crear el usuario administrador: " + userResult.Message);

                var adminUser = userResult.Data!;
                adminUser.AssignRole(adminRole); // asignar rol de administrador
                await _userRepository.AddAsync(adminUser);
            }
            await _unitOfWork.SaveChangesAsync();
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
