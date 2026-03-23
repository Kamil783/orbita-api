namespace Orbita.Infrastructure.Entities.Mapping;

public class TodoItemAssigneeEntity
{
    public Guid TodoItemId { get; set; }
    public TodoItemEntity TodoItem { get; set; } = default!;

    public Guid UserId { get; set; }
    public UserProfileEntity UserProfile { get; set; } = default!;
}
