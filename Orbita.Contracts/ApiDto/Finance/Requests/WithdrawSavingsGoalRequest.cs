namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class WithdrawSavingsGoalRequest
{
    public required long Amount { get; set; }
}