namespace Orbita.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddFrontCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Front", policy =>
                policy
                    .WithOrigins(
                        "http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );
        });

        return services;
    }
}
