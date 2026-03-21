namespace Orbita.Contracts.ApiDto.User.Responses;

public sealed class MemberDataResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public byte[] Avatar { get; set; } = [];
}
