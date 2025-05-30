using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("api/[controller]")]
public abstract class ApplicationController : ControllerBase;