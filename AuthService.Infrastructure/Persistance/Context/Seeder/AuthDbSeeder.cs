using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Domain;

namespace AuthService.Infrastructure.Persistance.Context.Seeder
{
    public class AuthDbSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthDbSeeder(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordService passwordService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
        }

        public async Task SeedAsync()
        {
            // 1. Roles predefinidos
            var adminRole = await EnsureRoleAsync("Admin");
            var userRole = await EnsureRoleAsync("User");

            // 2. Usuario administrador inicial
            var adminEmail = "admin@example.com";
            var existingAdmin = await _userRepository.GetByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var passwordHash = _passwordService.Hash("Admin123!"); // contraseña inicial segura
                var userResult = User.Create(
                    "admin",
                    adminEmail,
                    passwordHash,
                    "System",
                    "Admin"
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
