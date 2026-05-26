using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UserHub.Infrastructure.Settings;

namespace UserHub.Infrastructure.Messaging;

public sealed class RabbitMqConnection(IOptions<RabbitMqOptions> options) : IAsyncDisposable
{
    private readonly RabbitMqOptions _opt = options.Value;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IConnection? _connection;

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is {IsOpen : true}) return _connection;

        await _gate.WaitAsync(cancellationToken);

        try
        {
            if (_connection is {IsOpen : true}) return _connection;

            var factory = new ConnectionFactory
            {
                HostName = _opt.Host,
                Port = _opt.Port,
                UserName = _opt.Username,
                Password = _opt.Password
            };
            _connection = await factory.CreateConnectionAsync(cancellationToken);

            return _connection;
        } 
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null) await _connection.DisposeAsync();
        _gate.Dispose();
    }
}