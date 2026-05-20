namespace UserHub.Application.Abstractions.Persistence;

public interface INipGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken);
}