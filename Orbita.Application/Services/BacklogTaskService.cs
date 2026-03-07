using Orbita.Application.Abstractions.Services;
using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Services;

public class BacklogTaskService : IBacklogTaskService
{
    public async Task<Result<BacklogTask>> Create(CreateBacklogTaskCommand command)
    {
        throw new NotImplementedException();
    }
}
