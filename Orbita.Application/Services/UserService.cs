using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Dto;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.User.Responses;

namespace Orbita.Application.Services;

public class UserService(IIdentityUserGateway gateway) : IUserService
{
    public async Task<Result<UserDataResponse>> GetDataAsync(Guid userId, CancellationToken ct = default)
    {
        var userData = await gateway.GetDataByIdAsync(userId, ct);
        return BuildDataResult(userData);
    }

    public async Task<Result<UserDataResponse>> GetDataAsync(string email, CancellationToken ct = default)
    {
        var userData = await gateway.GetDataByEmailAsync(email, ct);
        return BuildDataResult(userData);
    }

    private static Result<UserDataResponse> BuildDataResult(UserData? userData)
    {
        if (userData is null)
            return Result<UserDataResponse>.NotFound("User not found");

        var result = new UserDataResponse
        {
            Name = userData.Name,
            Email = userData.Email
        };

        return Result<UserDataResponse>.Ok(result);
    }
}
