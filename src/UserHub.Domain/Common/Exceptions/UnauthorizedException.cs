namespace UserHub.Domain.Common.Exceptions;

public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException(string code, string message) : base(code, message) { }
}