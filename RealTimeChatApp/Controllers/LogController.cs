using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet("log")]
        public async Task<IActionResult> GetLogs(
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null)
        {
            try
            {
                // Validate the request parameters
                if (startTime == null) startTime = DateTime.Now.AddMinutes(-5); // Default: Current Timestamp - 5 minutes
                if (endTime == null) endTime = DateTime.Now; // Default: Current Timestamp

                if (startTime >= endTime)
                {
                    return BadRequest(new { error = "Invalid request parameters" });
                }

                // Call the service to get log entries
                var logs = _logService.GetLogs(startTime.Value, endTime.Value);

                if (logs == null || !logs.Any())
                {
                    return NotFound(new { error = "No logs found" });
                }

                return Ok(new { message = "Log list received successfully", logs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
