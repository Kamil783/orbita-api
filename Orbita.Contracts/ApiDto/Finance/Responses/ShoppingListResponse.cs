namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class ShoppingListResponse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string ListType { get; init; } = null!;
    public bool Pinned { get; init; }
    public long CreatedAt { get; init; }
    public List<ShoppingListItemResponse> Items { get; init; } = new();
}
