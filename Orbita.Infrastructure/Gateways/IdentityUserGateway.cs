using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Models.Dto;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Gateways;

public class IdentityUserGateway(UserManager<UserEntity> userManager) : IIdentityUserGateway
{
    public async Task<IReadOnlyList<AdminUserData>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await userManager.Users
            .Include(u => u.UserProfile)
            .ToListAsync(ct);

        var result = new List<AdminUserData>(users.Count);
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";
            result.Add(new AdminUserData(
                user.Id,
                user.UserProfile?.Name ?? "",
                user.Email ?? "",
                role,
                user.UserProfile?.AvatarData,
                null));
        }

        return result;
    }

    public async Task<UserData?> GetDataByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper(), ct);

        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        return new UserData(user.Id, user.Email ?? "", user.UserProfile?.Name ?? "", user.UserProfile?.AvatarData ?? [], role);
    }

    public async Task<UserData?> GetDataByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        return new UserData(user.Id, user.Email ?? "", user.UserProfile?.Name ?? "", user.UserProfile?.AvatarData ?? [], role);
    }
}
