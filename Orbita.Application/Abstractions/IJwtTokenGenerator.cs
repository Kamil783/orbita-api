using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}
