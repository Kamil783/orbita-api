using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IBacklogTaskService
{
    Task<Result<BacklogTask>> Create(CreateBacklogTaskCommand command);
}
