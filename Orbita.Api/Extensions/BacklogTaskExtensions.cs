using Orbita.Application.Commands.BacklogTasks;
using Orbita.Contracts.ApiDto.Tasks.Requests;

namespace Orbita.Api.Extensions;

public static class BacklogTaskExtensions
{
    public static CreateBacklogTaskCommand ToCommand(this CreateBacklogTaskRequest request)
    {
        return new CreateBacklogTaskCommand
        {
            Title = request.Title,
            Priority = request.Priority,
            DueDate = request.DueDate,
            EstimateMinutes = request.EstimateMinutes,
            Assignee = request.Assignee,
            Description = request.Description
        };
    }
}
