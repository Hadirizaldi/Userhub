using UserHub.Application.Abstractions.Auth;
using UserHub.Web.Auth;
using UserHub.Web.Common.Exceptions;

namespace UserHub.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddScoped<IClientInfoAccessor, HttpClientInfoAccessor>();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        return services;
    }
}
