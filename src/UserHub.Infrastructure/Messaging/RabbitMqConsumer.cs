using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserHub.Application.Users.Events;
using UserHub.Infrastructure.Settings;
using UserHub.Application.Abstractions.Messaging;

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
                if (!Guid.TryParse(ea.BasicProperties.MessageId, out var messageId))
                    throw new InvalidOperationException("MessageId is missing or invalid.");
                    
                using var scope = scopeFactory.CreateScope();
                var dedup = scope.ServiceProvider.GetRequiredService<IProcessedMessageStore>();
        
                if (await dedup.ExistsAsync(messageId, cancellationToken))
                {
                    logger.LogInformation("Message {MessageId} already processed, skipping.", messageId);
                    await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    return;
                }   
        
                var evt = JsonSerializer.Deserialize<UserRegisteredEvent>(ea.Body.Span)
                    ?? throw new InvalidOperationException("Invalid message payload.");
        
                var handler = scope.ServiceProvider.GetRequiredService<UserRegisteredHandler>();
                await handler.HandleAsync(evt, cancellationToken);
        
                await dedup.AddAsync(messageId, cancellationToken);
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process message, discarding (requeue=false).");
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }   
        };

        await _channel.BasicConsumeAsync(queue: _opt.Queue, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
        logger.LogInformation("RabbitMqConsumer is listening on queue {Queue}", _opt.Queue);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}