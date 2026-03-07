using AuthService.Application.Abstractions.Events;
using AuthService.Application.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Text;

namespace AuthService.Host.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHostServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration);
            services.AddRabbitMQ(configuration);
            services.AddSingleton<IEventPublisher, EventPublisher>();

            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

            return services;
        }

        private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionFactory>(_ =>
            new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"],
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "0"),
                UserName = configuration["RabbitMQ:User"],
                Password = configuration["RabbitMQ:Password"]
            });

            services.AddSingleton<IConnection>(sp =>
                sp.GetRequiredService<IConnectionFactory>().CreateConnection());

            services.AddSingleton<IModel>(sp =>
                sp.GetRequiredService<IConnection>().CreateModel());

            return services;
        }
    }
}
