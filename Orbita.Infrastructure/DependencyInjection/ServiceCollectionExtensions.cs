using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Contracts.Auth;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Gateways;
using Orbita.Infrastructure.Identity;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Infrastructure.Jobs;
using Orbita.Infrastructure.Logging;
using Orbita.Infrastructure.Notifications;
using Orbita.Infrastructure.Persistence;
using Orbita.Application.Abstractions.Services;
using Orbita.Infrastructure.Repositories;
using Orbita.Infrastructure.Services;

namespace Orbita.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
                               ?? throw new InvalidOperationException("Connection string 'Postgres' was not found.");

        services.AddDbContext<OrbitaDbContext>(options => options.UseNpgsql(connectionString));

        services.AddIdentity<UserEntity, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<OrbitaDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                         ?? throw new InvalidOperationException("JWT options were not found.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ClockSkew = TimeSpan.FromMinutes(1),
                NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    var accessToken = ctx.Request.Query["access_token"];
                    var path = ctx.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/hubs/notifications"))
                    {
                        ctx.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IIdentityAuthGateway, IdentityAuthGateway>();
        services.AddScoped<IIdentityUserGateway, IdentityUserGateway>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IBacklogTaskRepository, BacklogTaskRepository>();
        services.AddScoped<IColumnRepository, ColumnRepository>();
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IWeekRepository, WeekRepository>();
        services.AddScoped<IFinanceBalanceRepository, FinanceBalanceRepository>();
        services.AddScoped<IFinanceCategoryRepository, FinanceCategoryRepository>();
        services.AddScoped<IFinanceTransactionRepository, FinanceTransactionRepository>();
        services.AddScoped<ISavingsGoalRepository, SavingsGoalRepository>();
        services.AddScoped<ISpendingLimitRepository, SpendingLimitRepository>();
        services.AddScoped<ITeamCapacityRepository, TeamCapacityRepository>();
        services.AddScoped<IRecurringPaymentRepository, RecurringPaymentRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<ITeamProvider, TeamProvider>();
        services.AddScoped<IAppNotificationRepository, AppNotificationRepository>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSignalR();

        services.AddHttpContextAccessor();
        services.AddSingleton(Channel.CreateBounded<AppLogEntity>(new BoundedChannelOptions(10_000)
        {
            SingleReader = true,
            FullMode = BoundedChannelFullMode.DropOldest
        }));
        services.AddSingleton<IAppLogger, AppLogger>();
        services.AddHostedService<LogBackgroundService>();

        services.AddScoped<IDailyJob, MonthRolloverJob>();
        services.AddScoped<IDailyJob, WeekRolloverJob>();
        services.AddHostedService<DailyTaskRunnerService>();

        return services;
    }
}
