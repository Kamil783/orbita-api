using Orbita.Application.Models.Dto;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string Generate(AuthUserDto user);
}
