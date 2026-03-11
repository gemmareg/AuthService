using System.Text;
using System.Text.Json;
using AuthService.Application.Abstractions.Events;
using Auth.Contracts.Events;
using RabbitMQ.Client;

public class EventPublisher : IEventPublisher
{
    private readonly IModel _channel;

    public EventPublisher(IModel channel)
    {
        _channel = channel;

        // Declare exchange once
        _channel.ExchangeDeclare(
            exchange: "auth.events",
            type: ExchangeType.Topic,
            durable: true
        );
    }

    public void PublishUserRegistered(UserRegisteredEvent evt)
    {
        var json = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: "auth.events",
            routingKey: "user.registered",
            basicProperties: null,
            body: body
        );
    }

    public void PublishUserSoftDeleted(UserSoftDeletedEvent evt)
    {
        var json = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: "auth.events",
            routingKey: "user.soft-deleted",
            basicProperties: null,
            body: body
        );
    }
}
