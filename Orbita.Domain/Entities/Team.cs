using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class Team
{
    public TeamId Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<User> _teamMembers = [];
    public IReadOnlyCollection<User> TeamMembers => _teamMembers.AsReadOnly();

    private Team() { }

    public static Team Create()
    {
        return new Team
        {
            Id = new TeamId(Guid.NewGuid()),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Team Restore(
        Guid id, 
        DateTime createdAt, 
        DateTime updated, 
        IEnumerable<User> teamMembers)
    {
        var team = new Team
        {
            Id = new TeamId(id),
            CreatedAt = createdAt,
            UpdatedAt = updated
        };
        team._teamMembers.AddRange(teamMembers);
        return team;
    }
}
