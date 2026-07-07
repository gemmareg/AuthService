using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Behaviors;
using AuthService.Application.Extensions.Options;
using AuthService.Application.Security;
using AuthService.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AuthService.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<EventPublisherOptions>(configuration.GetSection("EventPublisherSeed"));

            services.AddScoped<ITokenGenerator, TokenService>();
            services.AddScoped<ITokenRefresher, TokenService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddSingleton<ICommonPasswordChecker, CommonPasswordChecker>();
            services.AddSingleton<IServiceTokenProvider, EventPublisherTokenProvider>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();

            return services;
        }
    }
}
