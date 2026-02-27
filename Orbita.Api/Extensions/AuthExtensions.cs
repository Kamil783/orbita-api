using Orbita.Application.Commands;
using Orbita.Contracts.ApiDto.User.Requests;

namespace Orbita.Api.Extensions;

public static class AuthExtensions
{
    public static LoginCommand ToCommand(this LoginRequest authDto)
    {
        return new LoginCommand
        {
            Email = authDto.Email,
            Password = authDto.Password
        };
    }

    public static RegisterCommand ToCommand(this RegisterRequest registerDto)
    {
        return new RegisterCommand
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };
    }
}
