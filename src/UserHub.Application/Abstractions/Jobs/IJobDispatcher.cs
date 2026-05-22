using System.Linq.Expressions;

namespace UserHub.Application.Abstractions.Jobs;

public interface IJobDispatcher
{
    string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall);

    string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay);
}
