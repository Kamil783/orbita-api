namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdateShoppingListItemDetailsRequest
{
    public string? Name { get; set; }
    public long? Price { get; set; }
}
