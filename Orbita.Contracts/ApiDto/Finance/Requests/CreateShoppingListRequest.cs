namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class CreateShoppingListRequest
{
    public required string Name { get; set; }
    public bool FromBalance { get; set; }
}
