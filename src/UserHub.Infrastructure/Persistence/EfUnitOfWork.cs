using Microsoft.EntityFrameworkCore.Storage;
using UserHub.Application.Abstractions.Persistence;

namespace UserHub.Infrastructure.Persistence;

public sealed class EfUnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken) =>
        new EfTransaction(await db.Database.BeginTransactionAsync(cancellationToken));

    private sealed class EfTransaction(IDbContextTransaction transaction) : ITransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) => transaction.CommitAsync(cancellationToken);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}