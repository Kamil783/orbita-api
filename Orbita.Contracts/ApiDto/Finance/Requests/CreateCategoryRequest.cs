namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class CreateCategoryRequest
{
    public required string Name { get; set; }
    public required string Icon { get; set; }
    public required string Bg { get; set; }
    public required string Color { get; set; }
    public long? WeeklyLimit { get; set; }
    public long? MonthlyLimit { get; set; }
}
