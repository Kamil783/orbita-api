using Orbita.Application.Models.Dto;

namespace Orbita.Application.Abstractions.Gateways;

public interface IIdentityUserGateway
{
    Task<UserData?> GetDataByEmailAsync(string email, CancellationToken ct = default);
    Task<UserData?> GetDataByIdAsync(Guid userId, CancellationToken ct = default);
}
