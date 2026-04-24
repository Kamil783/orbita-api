using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IRecurringPaymentRepository
{
    Task<List<RecurringPayment>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<RecurringPayment?> GetAsync(Guid id, CancellationToken ct = default);
    Task<RecurringPayment> CreateAsync(RecurringPayment payment, CancellationToken ct = default);
    Task<RecurringPayment> UpdateAsync(RecurringPayment payment, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
