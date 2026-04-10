namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdateShoppingListRequest
{
    public string? Name { get; set; }
    public bool? FromBalance { get; set; }
    public bool? Pinned { get; set; }
}
