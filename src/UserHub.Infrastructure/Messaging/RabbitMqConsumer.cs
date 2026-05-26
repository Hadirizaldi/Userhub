using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserHub.Application.Users.Events;
using UserHub.Infrastructure.Settings;

namespace UserHub.Infrastructure.Messaging;

public sealed class RabbitMqConsumer (
    RabbitMqConnection connection,
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqConsumer> logger
) : BackgroundService
{
    private readonly RabbitMqOptions _opt = options.Value;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var conn = await connection.GetConnectionAsync(cancellationToken);
        _channel = await conn.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            _opt.Exchange, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(
            _opt.Queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await _channel.QueueBindAsync( 
            _opt.Queue, _opt.Exchange, UserRegisteredEvent.RoutingKey, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var evt = JsonSerializer.Deserialize<UserRegisteredEvent>(ea.Body.Span)
                    ?? throw new InvalidOperationException("Pesan tidak valid.");

                using var scope = scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<UserRegisteredHandler>();
                await handler.HandleAsync(evt, cancellationToken);

                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gagal proses pesan, dibuang (requeue=false).");
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await _channel.BasicConsumeAsync(queue: _opt.Queue, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
        logger.LogInformation("RabbitMqConsumer mendengarkan queue {Queue}", _opt.Queue);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}