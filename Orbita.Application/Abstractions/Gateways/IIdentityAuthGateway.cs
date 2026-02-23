using Orbita.Application.Models.Dto;

namespace Orbita.Application.Abstractions.Gateways;

public interface IIdentityAuthGateway
{
    Task<AuthUserData?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<AuthUserData?> FindByIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken ct = default);
    Task<AuthUserData> CreateUserAsync(string email, string password, CancellationToken ct = default);
}
