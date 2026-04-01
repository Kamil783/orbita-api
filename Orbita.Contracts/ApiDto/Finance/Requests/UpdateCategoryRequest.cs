namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdateCategoryRequest
{
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Bg { get; set; }
    public string? Color { get; set; }
    public long? WeeklyLimit { get; set; }
    public long? MonthlyLimit { get; set; }
}
