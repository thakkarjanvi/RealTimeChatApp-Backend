using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System.Security.Claims;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Login failed due to validation errors" });
            }

            var userDto = await _userService.AuthenticateAsync(model);
            var token = _userService.GenerateJwtToken(userDto);

            if (userDto != null)
            {
                return Ok(new
                {
                    message = "Login successfully done",
                    token, // Get the actual JWT token from the authentication service
                    profile = userDto
                });
            }
            else
            {
                return Unauthorized(new { error = "Login failed due to incorrect credentials" });
            }
        }
        [HttpGet("users")]
        [Authorize] // Requires authentication to access this endpoint
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var users = await _userService.GetUsersAsync(userId);
                return Ok(new { message = "User list retrieved successfully", users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
