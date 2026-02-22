using Microsoft.Extensions.DependencyInjection;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Services;

namespace Orbita.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IAuthService, AuthService>();
        return services;
    }
}
