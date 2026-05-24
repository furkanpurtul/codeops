using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

namespace CodeOps.Api.Controllers.V1
{
    /// <summary>
    /// Example endpoints for validating API versioning and OpenAPI output.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ExamplesController : ControllerBase
    {
        /// <summary>
        /// Returns a simple example payload from version 1 of the API.
        /// </summary>
        /// <returns><see cref="ExampleResponse"/></returns>
        [HttpGet]
        [ProducesResponseType<ExampleResponse>(StatusCodes.Status200OK)]
        public ActionResult<ExampleResponse> Get()
        {
            return Ok(new ExampleResponse(
                Message: "CodeOps API is reachable.",
                Version: "v1",
                TimestampUtc: DateTimeOffset.UtcNow));
        }
    }

    /// <summary>
    /// Response contract for the example endpoint.
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Version"></param>
    /// <param name="TimestampUtc"></param>
    public sealed record ExampleResponse(string Message, string Version, DateTimeOffset TimestampUtc);
}