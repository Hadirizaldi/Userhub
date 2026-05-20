namespace UserHub.Domain.Common.Exceptions;

public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string code, string message) : base(code, message) { }
}
