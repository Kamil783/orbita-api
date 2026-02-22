using Orbita.Application.Models.Dto;

namespace Orbita.Application.Abstractions.Gateways;

public interface IIdentityAuthGateway
{
    Task<AuthUserDto?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<AuthUserDto?> FindByIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken ct = default);
    Task<AuthUserDto> CreateUserAsync(string email, string password, CancellationToken ct = default);
}
