namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class AddShoppingListItemRequest
{
    public required string Name { get; set; }
    public long? Price { get; set; }
}
