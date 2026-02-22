using Microsoft.AspNet.Identity;
using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Commands;
using Orbita.Application.Models.Dto;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.User.Responses;

namespace Orbita.Application.Services;

public class AuthService(IIdentityAuthGateway gateway, IJwtTokenGenerator jwt) : IAuthService
{
    public async Task<Result<AuthResponse>> AuthenticateAsync(LoginCommand command, CancellationToken ct = default)
    {
        var user = await gateway.FindByEmailAsync(command.Email, ct);
        if (user is null) 
            return Result<AuthResponse>.Fail("Invalid credentials");

        var ok = await gateway.CheckPasswordAsync(user.UserId, command.Password, ct);
        if (!ok) 
            return Result<AuthResponse>.Fail("Invalid credentials");

        var token = jwt.Generate(user);
        var result = new AuthResponse
        {
            AccessToken = token
        };

        return Result<AuthResponse>.Ok(result);
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
}