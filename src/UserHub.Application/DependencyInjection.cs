using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserHub.Application.Users.Commands.CreateUser;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Users.Policies;
using UserHub.Application.Users.Queries.GetUserById;
using UserHub.Application.Users.Commands.UpdateUser;
using UserHub.Application.Users.Commands.ChangeUserRole;
using UserHub.Application.Users.Commands.ChangeUserStatus;
using UserHub.Application.Users.Commands.ChangeUserPassword;
using UserHub.Application.Users.Commands.DeleteUser;
using UserHub.Application.Users.Commands.RestoreUser;
using UserHub.Application.Roles.Queries.LookupRoles;
using UserHub.Application.ConditionStatuses.Queries.LookupConditionStatuses;
using UserHub.Application.UserStatuses.Queries.LookupUserStatuses;

namespace UserHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

        services.AddSingleton<PasswordPolicy>();
        services.AddSingleton<PhonePolicy>();
        services.AddSingleton<RoleChangePolicy>();

        services.AddScoped<GetUsersService>();
        services.AddScoped<CreateUserService>();
        services.AddScoped<GetUserByIdService>();
        services.AddScoped<UpdateUserService>();
        services.AddScoped<LookupRolesService>();
        services.AddScoped<LookupConditionStatusService>();
        services.AddScoped<LookupUserStatusesService>();
        services.AddScoped<ChangeUserRoleService>();
        services.AddScoped<ChangeUserStatusService>();
        services.AddScoped<ChangeUserPasswordService>();
        services.AddScoped<DeleteUserService>();
        services.AddScoped<RestoreUserService>();

        return services;
    }
}
