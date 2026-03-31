namespace Orbita.Contracts.ApiDto.Team.Requests;

public class AddMemberRequest
{
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
}
