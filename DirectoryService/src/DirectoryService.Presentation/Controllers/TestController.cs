using DirectoryService.Application;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.EndpointResults;
using SharedService.SharedKernel.Models;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<Envelope<string>>(200)]
    public async Task<EndpointResult<string>> Test(
        [FromServices] TestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return await handler.Handle(
            new TestCommand(Guid.NewGuid()),
            cancellationToken);
    }
}