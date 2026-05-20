using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;

namespace UserHub.Infrastructure.Persistence;

public sealed class NipGenerator(AppDbContext db, IClock clock) : INipGenerator
{
    private static readonly TimeZoneInfo WibTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

    public async Task<string> GenerateAsync(CancellationToken cancellationToken)
    {
        var seq = await db.Database
            .SqlQueryRaw<long>("SELECT nextval('user_nip_seq') AS \"Value\"")
            .FirstAsync(cancellationToken);

        var wibNow = TimeZoneInfo.ConvertTimeFromUtc(clock.UtcNow, WibTimeZone);
        var datePart = wibNow.ToString("ddMMyyyy");

        return $"Sk-{datePart}-{seq}";
    }
}