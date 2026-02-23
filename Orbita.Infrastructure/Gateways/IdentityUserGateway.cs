using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Models.Dto;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Gateways;

public class IdentityUserGateway(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : IIdentityUserGateway
{
    public async Task<UserData?> GetDataByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return null;

        return new UserData(user.Id, user.Email ?? "", user.UserProfile?.Name ?? "");
    }

    public async Task<UserData?> GetDataByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);
        if (user is null) return null;

        return new UserData(user.Id, user.Email ?? "", user.UserProfile?.Name ?? "");
    }
}
