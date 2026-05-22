using Hangfire;
using Microsoft.Extensions.Options;
using UserHub.Application.Auth;
using UserHub.Application.Auth.Jobs;

namespace UserHub.Web.Extensions;

public static class HangfireJobsExtensions
{
    public static WebApplication UseRecurringJobs(this WebApplication app)
    {
        RegisterTokenCleanup(app);

        return app;
    }

    private static void RegisterTokenCleanup(WebApplication app)
    {
        var opt = app.Services.GetRequiredService<IOptions<CleanupOptions>>().Value;

        RecurringJob.AddOrUpdate<TokenCleanupJob>(
            "token-cleanup",
            job => job.RunAsync(CancellationToken.None),
            opt.CronExpression,
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(opt.TimeZone)
            });
    }
}
