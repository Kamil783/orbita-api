namespace Orbita.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}
