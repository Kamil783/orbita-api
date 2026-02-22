using Orbita.Application.Commands;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.User.Responses;

namespace Orbita.Application.Abstractions.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> AuthenticateAsync(LoginCommand command, CancellationToken ct = default);
    Task<Result<AuthResponse>> RegisterAsync(RegisterCommand command, CancellationToken ct = default);
}
