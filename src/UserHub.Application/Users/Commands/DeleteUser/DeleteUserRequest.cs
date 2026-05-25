    namespace UserHub.Application.Users.Commands.DeleteUser;

    public sealed record DeleteUserRequest(
        bool? Force
    );
