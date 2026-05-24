using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class PlannedPurchase
{
    public PlannedPurchaseId Id { get; private set; }
    public UserId OwnerId { get; private set; }
    public TeamId TeamId { get; private set; }
    public string Title { get; private set; }
    public DateOnly Date { get; private set; }
    public long Amount { get; private set; }

    /// <summary>Тип исполнителя. null — не назначен.</summary>
    public PlannedPurchaseAssigneeKind? AssigneeKind { get; private set; }
    /// <summary>Конкретный пользователь-исполнитель. Имеет смысл только при <see cref="AssigneeKind"/> = User.</summary>
    public UserId? AssigneeUserId { get; private set; }

    public FinanceCategoryId? CategoryId { get; private set; }
    public string? Note { get; private set; }
    public PlannedPurchaseStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private PlannedPurchase() { }

    public static PlannedPurchase Create(
        UserId ownerId,
        TeamId teamId,
        string title,
        DateOnly date,
        long amount,
        PlannedPurchaseAssigneeKind? assigneeKind,
        UserId? assigneeUserId,
        FinanceCategoryId? categoryId,
        string? note)
    {
        ValidateTitle(title);
        ValidateAmount(amount);
        ValidateAssignee(assigneeKind, assigneeUserId);

        var now = DateTime.UtcNow;
        return new PlannedPurchase
        {
            Id = new PlannedPurchaseId(Guid.NewGuid()),
            OwnerId = ownerId,
            TeamId = teamId,
            Title = title.Trim(),
            Date = date,
            Amount = amount,
            AssigneeKind = assigneeKind,
            AssigneeUserId = assigneeKind == PlannedPurchaseAssigneeKind.User ? assigneeUserId : null,
            CategoryId = categoryId,
            Note = NormalizeNote(note),
            Status = PlannedPurchaseStatus.Planned,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static PlannedPurchase Restore(
        PlannedPurchaseId id,
        UserId ownerId,
        TeamId teamId,
        string title,
        DateOnly date,
        long amount,
        PlannedPurchaseAssigneeKind? assigneeKind,
        UserId? assigneeUserId,
        FinanceCategoryId? categoryId,
        string? note,
        PlannedPurchaseStatus status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new PlannedPurchase
        {
            Id = id,
            OwnerId = ownerId,
            TeamId = teamId,
            Title = title,
            Date = date,
            Amount = amount,
            AssigneeKind = assigneeKind,
            AssigneeUserId = assigneeUserId,
            CategoryId = categoryId,
            Note = note,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    /// <summary>
    /// PATCH с двойной семантикой:
    ///   * Не-nullable поля (<paramref name="title"/>, <paramref name="date"/>, <paramref name="amount"/>,
    ///     <paramref name="status"/>): null = не трогать.
    ///   * Nullable поля (assignee, <paramref name="categoryId"/>, <paramref name="note"/>):
    ///     значение из вызова всегда записывается — null означает «снять».
    /// </summary>
    public void Update(
        string? title,
        DateOnly? date,
        long? amount,
        PlannedPurchaseAssigneeKind? assigneeKind,
        UserId? assigneeUserId,
        FinanceCategoryId? categoryId,
        string? note,
        PlannedPurchaseStatus? status)
    {
        if (title is not null)
        {
            ValidateTitle(title);
            Title = title.Trim();
        }

        if (date.HasValue)
            Date = date.Value;

        if (amount.HasValue)
        {
            ValidateAmount(amount.Value);
            Amount = amount.Value;
        }

        ValidateAssignee(assigneeKind, assigneeUserId);
        AssigneeKind = assigneeKind;
        AssigneeUserId = assigneeKind == PlannedPurchaseAssigneeKind.User ? assigneeUserId : null;

        CategoryId = categoryId;
        Note = NormalizeNote(note);

        if (status.HasValue)
            Status = status.Value;

        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (title.Trim().Length > 200)
            throw new ArgumentException("Title is too long (max 200).", nameof(title));
    }

    private static void ValidateAmount(long amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
    }

    private static void ValidateAssignee(PlannedPurchaseAssigneeKind? kind, UserId? userId)
    {
        if (kind == PlannedPurchaseAssigneeKind.User && userId is null)
            throw new ArgumentException("AssigneeUserId is required when AssigneeKind = User.", nameof(userId));
    }

    private static string? NormalizeNote(string? note)
    {
        if (note is null) return null;
        var trimmed = note.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }
}
