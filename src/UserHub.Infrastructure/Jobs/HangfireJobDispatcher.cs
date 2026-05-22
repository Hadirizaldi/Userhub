using System.Linq.Expressions;
using Hangfire;
using UserHub.Application.Abstractions.Jobs;

namespace UserHub.Infrastructure.Jobs;

public sealed class HangfireJobDispatcher(IBackgroundJobClient client) : IJobDispatcher
{
    public string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall) =>
        client.Enqueue(methodCall);

    public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay) =>
        client.Schedule(methodCall, delay);
}
