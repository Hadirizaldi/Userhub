using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserHub.Infrastructure.Persistence;
using UserHub.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Infrastructure.Persistence.Repositories;
using UserHub.Application.Abstractions.Security;
using UserHub.Application.Abstractions.Time;
using UserHub.Infrastructure.Security;
using UserHub.Infrastructure.Time;
using UserHub.Infrastructure.Auth;
using UserHub.Application.Abstractions.Auth;

namespace UserHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var db = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(db.Postgresql.Core);
        });

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

        services.AddSingleton<ReferenceDataCatalog>();
        services.AddSingleton<IReferenceDataCatalog>(sp => sp.GetRequiredService<ReferenceDataCatalog>());
        services.AddHostedService(sp => sp.GetRequiredService<ReferenceDataCatalog>());
        
        services.AddScoped<INipGenerator, NipGenerator>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IConditionStatusRepository, ConditionStatusRepository>();
        services.AddScoped<IUserStatusRepository, UserStatusRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        return services;
    }
}
