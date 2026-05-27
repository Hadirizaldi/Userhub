using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UserHub.Application.Abstractions.Messaging;
using UserHub.Infrastructure.Settings;

namespace UserHub.Infrastructure.Messaging;

public sealed class RabbitMqEventPublisher (
    RabbitMqConnection connection,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqEventPublisher> logger
) : IEventPublisher
{
    private readonly RabbitMqOptions _opt = options.Value;

    public async Task PublishAsync(string routingKey, string payload, Guid messageId,CancellationToken cancellationToken)
    {
        try
        {
            var conn = await connection.GetConnectionAsync(cancellationToken);
            await using var channel = await conn.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: _opt.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                cancellationToken: cancellationToken
            );

            var body = System.Text.Encoding.UTF8.GetBytes(payload);
            var props = new BasicProperties
            {
                MessageId = messageId.ToString(),
                DeliveryMode = DeliveryModes.Persistent
            };

            await channel.BasicPublishAsync(
                exchange: _opt.Exchange,
                routingKey: routingKey,
                basicProperties: props,
                body: body,
                mandatory: false,
                cancellationToken: cancellationToken
            );

            logger.LogInformation("Published event {RoutingKey} ({MessageId})", routingKey, messageId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish {RoutingKey} ({MessageId})", routingKey, messageId);
            throw;
        }
    }
}