namespace Orbita.Contracts.ApiDto.Admin.Responses;

public sealed class AdminUserResponse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public string? Avatar { get; init; }
    public string? CreatedAt { get; init; }
}
