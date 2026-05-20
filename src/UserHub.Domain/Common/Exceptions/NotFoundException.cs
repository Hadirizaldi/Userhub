namespace UserHub.Domain.Common.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string code, string message) : base(code, message) {}

        public static NotFoundException For(string resource, object id) =>
            new($"{resource.ToUpperInvariant()}_NOT_FOUND", $"{resource} with id '{id}' was not found.");
}