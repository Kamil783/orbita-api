using Microsoft.Extensions.DependencyInjection;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Services;

namespace Orbita.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IBacklogTaskService, BacklogTaskService>();
        services.AddTransient<IColumnService, ColumnService>();
        services.AddTransient<ITodoItemService, TodoItemService>();
        services.AddTransient<IWeekService, WeekService>();
        services.AddTransient<IFinanceService, FinanceService>();
        services.AddTransient<IAdminService, AdminService>();
        services.AddTransient<ITeamService, TeamService>();
        return services;
    }
}
