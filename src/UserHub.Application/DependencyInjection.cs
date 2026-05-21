using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserHub.Application.Users.Commands.CreateUser;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Users.Policies;
using UserHub.Application.Users.Queries.GetUserById;
using UserHub.Application.Users.Commands.UpdateUser;
using UserHub.Application.Users.Commands.ChangeUserRole;
using UserHub.Application.Roles.Queries.LookupRoles;
using UserHub.Application.ConditionStatuses.Queries.LookupConditionStatuses;

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
        services.AddScoped<ChangeUserRoleService>();

        return services;
    }
}
