using UserHub.Application.Auth;

namespace UserHub.Application.Abstractions.Auth;

public interface IClientInfoAccessor
{
    ClientInfo Current { get; }
}