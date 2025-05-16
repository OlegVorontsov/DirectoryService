using DirectoryService.Application;
using DirectoryService.Application.Test;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.EndpointResults;
using SharedService.SharedKernel.Models;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpPost("{testId:guid}")]
    [ProducesResponseType<Envelope<string>>(200)]
    public async Task<EndpointResult<string>> Test(
        [FromRoute] Guid testId,
        [FromBody] TestRequest request,
        [FromServices] TestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return await handler.Handle(
            request.ToCommand(testId),
            cancellationToken);
    }
}