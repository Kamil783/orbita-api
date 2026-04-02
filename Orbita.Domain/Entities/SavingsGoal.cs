using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class SavingsGoal
{
    public SavingsGoalId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public string Name { get; private set; }
    public long Target { get; private set; }
    public long Current { get; private set; }

    private SavingsGoal() { }

    public static SavingsGoal Create(
        UserId creatorId,
        TeamId teamId,
        string name,
        long target)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (target <= 0)
            throw new ArgumentOutOfRangeException(nameof(target), "Target must be positive.");

        return new SavingsGoal
        {
            Id = new SavingsGoalId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name,
            Target = target,
            Current = 0
        };
    }

    public static SavingsGoal Restore(
        SavingsGoalId id,
        UserId creatorId,
        TeamId teamId,
        string name,
        long target,
        long current)
    {
        return new SavingsGoal
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name,
            Target = target,
            Current = current
        };
    }

    public void AddFunds(long amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        Current += amount;
    }

    public void WithdrawFunds(long amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        if (amount > Current)
            throw new InvalidOperationException("Insufficient funds.");

        Current -= amount;
    }
}
