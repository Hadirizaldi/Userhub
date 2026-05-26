using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Security;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Auth;
using UserHub.Infrastructure.Auth;
using UserHub.Application.Abstractions.Jobs;
using UserHub.Infrastructure.Jobs;
using UserHub.Infrastructure.Persistence;
using UserHub.Infrastructure.Persistence.Repositories;
using UserHub.Infrastructure.Security;
using UserHub.Infrastructure.Settings;
using UserHub.Infrastructure.Time;
using UserHub.Application.Abstractions.Audit;
using UserHub.Infrastructure.Audit;
using UserHub.Application.Abstractions.Messaging;
using UserHub.Infrastructure.Messaging;

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

        services.AddOptions<CleanupOptions>()
            .Bind(configuration.GetSection(CleanupOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<HangfireOptions>()
            .Bind(configuration.GetSection(HangfireOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var dbOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? throw new InvalidOperationException("DbContextSettings configuration is missing.");

        var hangfireOptions = configuration.GetSection(HangfireOptions.SectionName).Get<HangfireOptions>()
            ?? throw new InvalidOperationException("Hangfire configuration is missing.");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(dbOptions.Postgresql.Core),
                new PostgreSqlStorageOptions
                {
                    SchemaName = hangfireOptions.SchemaName,
                    PrepareSchemaIfNecessary = hangfireOptions.PrepareSchemaIfNecessary,
                    QueuePollInterval = TimeSpan.FromSeconds(hangfireOptions.PollingIntervalSeconds)
                }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = hangfireOptions.WorkerCount;
            options.Queues = new[] { "default" };
        });

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var db = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(db.Postgresql.Core);
        });

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<ReferenceDataCatalog>();
        services.AddSingleton<IReferenceDataCatalog>(sp => sp.GetRequiredService<ReferenceDataCatalog>());
        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        services.AddHostedService<RabbitMqConsumer>();
        services.AddHostedService(sp => sp.GetRequiredService<ReferenceDataCatalog>());

        services.AddScoped<INipGenerator, NipGenerator>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IConditionStatusRepository, ConditionStatusRepository>();
        services.AddScoped<IUserStatusRepository, UserStatusRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IJobDispatcher, HangfireJobDispatcher>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddScoped<IAuditLogReader, AuditLogReader>();

        return services;
    }
}
