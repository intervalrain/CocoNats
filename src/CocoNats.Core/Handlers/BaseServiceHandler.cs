
using CocoNats.Abstractions.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NATS.Client.Core;

namespace CocoNats.Core.Handlers;

/// <summary>
/// Base handler for NATS services that automatically handles subscription and reply logic.
/// </summary>
public abstract class BaseServiceHandler<TService>(
    ILogger<BaseServiceHandler<TService>> logger,
    INatsConnection connection,
    IServiceProvider serviceProvider
) : IHostedService, IAsyncDisposable
    where TService : class, INatsService
{
    private readonly ILogger<BaseServiceHandler<TService>> _logger = logger;
    private readonly INatsConnection _connection = connection;
    public IServiceProvider ServiceProvider { get; }= serviceProvider;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}