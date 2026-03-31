namespace Orbita.Infrastructure.Entities;

public class TeamEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<UserEntity> TeamMembers { get; set; } = [];
}
