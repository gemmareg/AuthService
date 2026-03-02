using AuthService.Application.Abstractions.Events;
using RabbitMQ.Client;

namespace AuthService.Host.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHostServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMQ(configuration);
            services.AddSingleton<IEventPublisher, EventPublisher>();

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
