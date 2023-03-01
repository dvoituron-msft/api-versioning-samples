using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApiVersioning.Controllers;

[ApiController]
[ApiVersion(ApiVersions.V1_0)]
[ApiVersion(ApiVersions.V2_0)]
[ApiVersion(ApiVersions.V3_0)]
[ApiVersion(ApiVersions.V4_0)]
[Route("api/HelloWorld")]
[Produces("application/json")]
public class HelloWorldController : ControllerBase
{
    // Available in all versions
    [HttpGet]
    public async Task<ActionResult<string>> GetAsync()
    {
        await Task.CompletedTask;
        
        var version = HttpContext.GetRequestedApiVersion();
        switch (version?.MajorVersion)
        {
            case 1:
            case 2:
            case 3:
                return Ok($"GET - Hello world v{version.MajorVersion}.");

            case 4:
                return Ok($"GET - Hello world v4.");

            default:
                return NotFound();
        }            
    }
    
    [HttpPut]
    [MapToApiVersion(ApiVersions.V2_0)]
    public async Task<ActionResult<string>> PutAsync()
    {
        await Task.CompletedTask;
        return Ok("PUT - Hello world v2.");
    }

    [HttpPut]
    [MapToApiVersion(ApiVersions.V3_0)]
    [MapToApiVersion(ApiVersions.V4_0)]
    public async Task<ActionResult<string>> PutV3Async()
    {
        await Task.CompletedTask;
        return Ok("PUT - Hello world from v3 and v4.");
    }

    [HttpPost]
    [MapToApiVersion(ApiVersions.V3_0)]
    public async Task<ActionResult<string>> PostV3Async()
    {
        await Task.CompletedTask;
        return Ok("POST - Hello world v3.");
    }
    
    [HttpPost]
    [MapToApiVersion(ApiVersions.V4_0)]
    public async Task<ActionResult<string>> PostV4Async()
    {
        await Task.CompletedTask;
        return Ok("POST - Hello world v4.");
    }
}

[ApiController]
[ApiVersion(ApiVersions.V0_5, Deprecated = true)]
[Route("api/HelloWorld")]
public class HelloWorldDeprecatedController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> GetV0Async()
    {
        await Task.CompletedTask;
        return Ok("GET - Hello world v0.5");
    }
}
