using Microsoft.Extensions.Options;
using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Commands;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.User.Responses;
using Orbita.Contracts.Auth;

namespace Orbita.Application.Services;

public class AuthService(
    IIdentityAuthGateway gateway,
    IJwtTokenGenerator jwt,
    IRefreshTokenRepository refreshTokens,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<AuthResponse>> AuthenticateAsync(LoginCommand command, CancellationToken ct = default)
    {
        var user = await gateway.FindByEmailAsync(command.Email, ct);
        if (user is null)
            return Result<AuthResponse>.Unauthorized("Invalid credentials");

        var ok = await gateway.CheckPasswordAsync(user.UserId, command.Password, ct);
        if (!ok)
            return Result<AuthResponse>.Unauthorized("Invalid credentials");

        var accessToken = jwt.Generate(user);
        var refreshToken = jwt.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays);

        await refreshTokens.AddAsync(refreshToken, user.UserId, expiresAt, ct);

        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterCommand command, CancellationToken ct = default)
    {
        await gateway.CreateUserAsync(command.Email, command.Password, ct);
        var loginCommand = new LoginCommand
        {
            Email = command.Email,
            Password = command.Password
        };

        return await AuthenticateAsync(loginCommand, ct);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenData = await refreshTokens.GetByTokenAsync(refreshToken, ct);

        if (tokenData is null || tokenData.IsRevoked || tokenData.ExpiresAt < DateTime.UtcNow)
            return Result<AuthResponse>.Unauthorized("Invalid or expired refresh token");

        var user = await gateway.FindByIdAsync(tokenData.UserId, ct);
        if (user is null)
            return Result<AuthResponse>.Unauthorized("Invalid or expired refresh token");

        await refreshTokens.RevokeAsync(refreshToken, ct);

        var accessToken = jwt.Generate(user);
        var newRefreshToken = jwt.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays);

        await refreshTokens.AddAsync(newRefreshToken, user.UserId, expiresAt, ct);

        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        });
    }
}
