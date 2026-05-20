namespace UserHub.Domain.Common.Exceptions;

public sealed class ConflictException : DomainException
{
    public ConflictException(string code, string message) : base(code, message) { }
}
