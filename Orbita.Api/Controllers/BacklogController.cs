using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Contracts.ApiDto.Tasks.Requests;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BacklogController(IBacklogTaskService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBacklog(CreateBacklogTaskRequest request)
    {
        var res = await service.Create(request.ToCommand());

        return res.ToActionResult(HttpContext);
    }
}
