using Microsoft.AspNetCore.Mvc;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("exception")]
        public IActionResult ThrowException()
        {
            throw new Exception("Something went wrong in the server!"); // Exception
        }

        [HttpGet("ok")]
        public IActionResult OkRequest()
        {
            return Ok(new { message = "Everything is fine" });
        }
    }
}

