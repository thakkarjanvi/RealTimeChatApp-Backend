using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApp.Domain.Models;
using System.Net;
using System.Security.Claims;
using RealTimeChatApp.Domain.Interfaces;

namespace RealTimeChatApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessage sendMessage)
        {
            try
            {
                // Check if the required parameters are provided
                if (sendMessage == null || sendMessage.ReceiverId == Guid.Empty || string.IsNullOrWhiteSpace(sendMessage.Content))
                {
                    return BadRequest(new { error = "Message sending failed due to validation errors" });
                }

                // Get the authenticated user's ID from the token
                var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(senderId))
                {
                    return Unauthorized(new { error = "Unauthorized access" });
                }

                // Call the service to send the message
                var messageDto = await _messageService.SendMessageAsync(new Guid(senderId), sendMessage);

                // Return a successful response with the created message
                return Ok(new
                {
                    Message = "Message sent successfully",
                    MessageId = messageDto.MessageId,
                    SenderId = messageDto.SenderId,
                    ReceiverId = messageDto.ReceiverId,
                    Content = messageDto.Content,
                    Timestamp = messageDto.Timestamp
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }
    }
}
