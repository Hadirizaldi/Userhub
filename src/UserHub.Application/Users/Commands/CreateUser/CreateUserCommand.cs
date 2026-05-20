using MediatR;
using UserHub.Application.Users.Queries.GetUsers;

namespace UserHub.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand (
    string Fullname,
    string Email,
    string Password,
    string? Phone
) : IRequest<UserListItemDto>;