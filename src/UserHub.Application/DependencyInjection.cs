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
using UserHub.Application.Users.Commands.BulkAssignRoles;
using UserHub.Application.Roles.Commands.CreateRole;
using UserHub.Application.Roles.Commands.UpdateRole;
using UserHub.Application.Roles.Commands.DeleteRole;
using UserHub.Application.Roles.Queries.GetRoles;
using UserHub.Application.Roles.Queries.GetRoleById;
using UserHub.Domain.Roles.Policies;
using UserHub.Application.Roles.Queries.LookupRoles;
using UserHub.Application.ConditionStatuses.Queries.LookupConditionStatuses;
using UserHub.Application.UserStatuses.Queries.LookupUserStatuses;
using UserHub.Application.Auth;
using Microsoft.Extensions.Configuration;
using UserHub.Application.Auth.Commands.Login;
using UserHub.Application.Auth.Commands.Logout;
using UserHub.Application.Auth.Commands.LogoutAll;
using UserHub.Application.Auth.Queries.GetMe;
using UserHub.Application.Auth.Queries.GetSessions;

namespace UserHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

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
        services.AddScoped<BulkAssignRolesService>();

        services.AddSingleton<RoleProtectionPolicy>();
        services.AddScoped<GetRolesService>();
        services.AddScoped<GetRoleByIdService>();
        services.AddScoped<CreateRoleService>();
        services.AddScoped<UpdateRoleService>();
        services.AddScoped<DeleteRoleService>();
        services.AddScoped<LoginService>();
        services.AddScoped<LogoutService>();
        services.AddScoped<LogoutAllService>();
        services.AddScoped<GetMeService>();
        services.AddScoped<GetSessionsService>();

        return services;
    }
}
