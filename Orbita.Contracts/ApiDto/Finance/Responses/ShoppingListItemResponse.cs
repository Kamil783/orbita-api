namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class ShoppingListItemResponse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public long? Price { get; init; }
    public bool Bought { get; init; }
}
