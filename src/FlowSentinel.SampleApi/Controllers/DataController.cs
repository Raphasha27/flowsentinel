using Microsoft.AspNetCore.Mvc;

namespace FlowSentinel.SampleApi.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    [HttpGet]
    public IActionResult GetData()
    {
        return Ok(new { Message = "Sensitive data retrieved successfully", Timestamp = DateTime.UtcNow });
    }

    [HttpPost("secure")]
    public IActionResult SecureOperation()
    {
        return Ok(new { Message = "High-value transaction completed." });
    }
}
