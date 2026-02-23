using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence;

public static class SeedDatabase
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var config = sp.GetRequiredService<IConfiguration>();

        var db = sp.GetRequiredService<OrbitaDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = sp.GetRequiredService<UserManager<UserEntity>>();

        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var res = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                if (!res.Succeeded)
                    throw new Exception($"Не удалось создать роль {role}: {string.Join(", ", res.Errors.Select(e => e.Description))}");
            }
        }

        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];
        var name = config["SeedAdmin:Name"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }


        var admin = await userManager.FindByEmailAsync(email);

        if (admin is null)
        {
            admin = new UserEntity
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createRes = await userManager.CreateAsync(admin, password);
            if (!createRes.Succeeded)
                throw new Exception($"Не удалось создать пользователя: {string.Join(", ", createRes.Errors.Select(e => e.Description))}");
        }

        var userRoles = await userManager.GetRolesAsync(admin);
        var needRoles = new[] { "Admin" };

        foreach (var role in needRoles.Except(userRoles))
        {
            var addRes = await userManager.AddToRoleAsync(admin, role);
            if (!addRes.Succeeded)
                throw new Exception($"Не удалось добавить роль {role}: {string.Join(", ", addRes.Errors.Select(e => e.Description))}");
        }
    }
}
