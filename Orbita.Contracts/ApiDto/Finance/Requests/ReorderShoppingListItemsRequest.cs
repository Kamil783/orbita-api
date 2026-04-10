namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class ReorderShoppingListItemsRequest
{
    public required List<Guid> ItemIds { get; set; }
}
