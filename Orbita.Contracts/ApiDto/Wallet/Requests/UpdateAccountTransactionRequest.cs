namespace Orbita.Contracts.ApiDto.Wallet.Requests;

/// <summary>
/// PATCH-семантика:
///  * Не-nullable поля (<see cref="Title"/>, <see cref="Amount"/>, <see cref="Date"/>): null = не трогать.
///  * Nullable поля (<see cref="CategoryId"/>): значение из тела всегда применяется; null = снять.
/// Смена счёта не поддерживается — у счетов могут быть разные валюты, нужно удалить и создать заново.
/// </summary>
public sealed class UpdateAccountTransactionRequest
{
    public string? Title { get; set; }
    public decimal? Amount { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime? Date { get; set; }
}
