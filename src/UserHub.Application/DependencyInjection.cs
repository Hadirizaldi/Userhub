using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserHub.Application.Users.Commands.CreateUser;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Users.Policies;
using UserHub.Application.Users.Queries.GetUserById;

namespace UserHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

        services.AddSingleton<PasswordPolicy>();
        services.AddSingleton<PhonePolicy>();

        services.AddScoped<GetUsersService>();
        services.AddScoped<CreateUserService>();
        services.AddScoped<GetUserByIdService>();

        return services;
    }
}
