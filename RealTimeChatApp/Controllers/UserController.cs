using Microsoft.AspNetCore.Mvc;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;

namespace RealTimeChatApp.Controllers
{
   [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { error = "Invalid registration request" });
            }

            var result = await _userService.RegisterAsync(request);

            if (result != null)
            {
                return Ok(new
                {
                    message = "Registration successful",
                    userId = result.Id,
                    fullname = result.Name,
                    email = result.Email
                });
            }
            else
            {
                return Conflict(new { error = "Registration failed because the email is already registered or user creation failed" });
            }
        }
    }
}
