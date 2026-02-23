using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.User.Responses;

namespace Orbita.Application.Abstractions.Services;

public interface IUserService
{
    Task<Result<UserDataResponse>> GetDataAsync(Guid userId, CancellationToken ct = default);
    Task<Result<UserDataResponse>> GetDataAsync(string email, CancellationToken ct = default);
}
