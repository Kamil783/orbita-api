using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class Team
{
    public TeamId Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<User> _teamMembers = [];
    public IReadOnlyCollection<User> TeamMembers => _teamMembers.AsReadOnly();

    private Team() { }

    public static Team Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        return new Team
        {
            Id = new TeamId(Guid.NewGuid()),
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Team Restore(
        Guid id,
        string name,
        DateTime createdAt,
        DateTime updatedAt,
        IEnumerable<User> teamMembers)
    {
        var team = new Team
        {
            Id = new TeamId(id),
            Name = name,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        team._teamMembers.AddRange(teamMembers);
        return team;
    }
}
