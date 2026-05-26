using System.Text.Json;
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

    public async Task PublishAsync<T>(string routingKey, T @event, CancellationToken cancellationToken)
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

            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            var props = new BasicProperties
            {
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

            logger.LogInformation("Published event {RoutingKey}", routingKey);
        }
        catch (Exception ex)
        {
            // Tahap 1: fire-and-forget. Broker error tidak boleh gagalin operasi bisnis.
            // Reliability sesungguhnya nanti di Outbox (Tahap 3).

            logger.LogWarning(ex, "Gagal publish {RoutingKey}, diabaikan.", routingKey);
        }
    }
}