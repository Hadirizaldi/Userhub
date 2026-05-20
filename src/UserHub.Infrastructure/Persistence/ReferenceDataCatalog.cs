using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserHub.Application.Abstractions.Persistence;

namespace UserHub.Infrastructure.Persistence;

public sealed class ReferenceDataCatalog(IServiceScopeFactory scopeFactory) : IHostedService, IReferenceDataCatalog
{
    public int ActiveUserStatusId {get; private set;}
    public int ActiveConditionStatusId {get; private set;}

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ActiveUserStatusId = await db.UserStatuses
            .Where(x => x.Name == "active")
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException();

        ActiveConditionStatusId = await db.ConditionStatuses
            .Where(x => x.Name == "active")
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}