namespace Orbita.Contracts.ApiDto.Wallet.Requests;

public sealed class CreateAccountTransactionRequest
{
    public required Guid AccountId { get; set; }
    public required string Title { get; set; }
    /// <summary>Знаковая сумма: отрицательное — расход, положительное — поступление. Не может быть 0.</summary>
    public required decimal Amount { get; set; }
    public Guid? CategoryId { get; set; }
    /// <summary>Дата операции. null — сейчас (UTC).</summary>
    public DateTime? Date { get; set; }
}
