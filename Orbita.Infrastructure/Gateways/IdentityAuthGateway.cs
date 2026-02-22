using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Models.Dto;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Gateways;

public class IdentityAuthGateway(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : IIdentityAuthGateway
{
    public async Task<AuthUserDto?> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return new AuthUserDto(user.Id, user.Email ?? email, [.. roles]);
    }

    public async Task<AuthUserDto?> FindByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);
        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return new AuthUserDto(user.Id, user.Email ?? "", [.. roles]);
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken ct = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);
        if (user is null) return false;

        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        return result.Succeeded;
    }

    public async Task<AuthUserDto> CreateUserAsync(string email, string password, CancellationToken ct = default)
    {
        var user = new UserEntity { UserName = email, Email = email };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException(errors);
        }

        var roles = await userManager.GetRolesAsync(user);
        return new AuthUserDto(user.Id, user.Email ?? email, [.. roles]);
    }
}
