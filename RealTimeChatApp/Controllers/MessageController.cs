using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApp.Domain.Models;
using System.Net;
using System.Security.Claims;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.DAL.Services;

namespace RealTimeChatApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IGenericRepository _genericRepository;

        public MessageController(IMessageService messageService, IGenericRepository genericRepository)
        {
            _messageService = messageService;
            _genericRepository = genericRepository;
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
        [HttpPut("messages/{messageId}")]
        public async Task<IActionResult> EditMessage(int messageId, [FromBody] EditMessage editMessage)
        {
            // Check if the editMessage object is provided in the request body
            if (editMessage == null || string.IsNullOrWhiteSpace(editMessage.Content))
            {
                return BadRequest(new { error = "Invalid or empty message content" });
            }

            // Get the authenticated user's ID from the claims
            var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (senderIdClaim == null || !Guid.TryParse(senderIdClaim.Value, out Guid senderId))
            {
                return Unauthorized(new { error = "Invalid user authentication" });
            }

            // Call the service to edit the message
            var editedMessage = await _messageService.EditMessageAsync(messageId, senderId, editMessage);

            // Check if the message was found
            if (editedMessage == null)
            {
                return NotFound(new { error = "Message not found" });
            }

            // Check if the authenticated user is the sender of the message
            if (editedMessage.SenderId != senderId)
            {
                return Unauthorized(new { error = "You are not authorized to edit this message" });
            }

            // Return a successful response with the edited message details
            return Ok(new
            {
                Message = "Message edited successfully",
                MessageId = editedMessage.MessageId,
                editedMessage.SenderId,
                editedMessage.ReceiverId,
                editedMessage.Content,
                editedMessage.Timestamp
                // Include other properties as needed
            });
        }

        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (senderIdClaim == null || !Guid.TryParse(senderIdClaim.Value, out Guid senderId))
                {
                    return Unauthorized(new { error = "Invalid user authentication" });
                }

                var result = await _messageService.DeleteMessageAsync(messageId, senderId);

                if (result == null)
                {
                    return NotFound(new { error = "Message not found" });
                }

                if (result.SenderId != senderId)
                {
                    return Unauthorized(new { error = "You are not authorized to delete this message" });
                }

                return Ok(new { Message = "Message deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet("messages")]
        public async Task<IActionResult> RetrieveConversationHistory([FromQuery] ConversationHistoryDto queryParameters)
        {
            try
            {
                // check the model Validations
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Invalid request parameters." });
                }

                // Get the current user's ID from the JWT token
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var messages = await _messageService.RetrieveConversationHistory(queryParameters, currentUserId);

                if (messages == null || messages.Count <= 0)
                {
                    return Ok(new { error = "No more conversation found." });
                }

                return Ok(new { message = "Conversation history retrieved successfully", messages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("converstaion/search")]
        public async Task<IActionResult> SearchConversationAsync([FromQuery] string query)
        {
            try
            {
                // Get the current user's ID from the claims
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var currentUserId))
                {
                    return Unauthorized(new { error = "User not authenticated or invalid user ID" });
                }

                // Validate the request parameters
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { error = "Invalid query parameter" });

                }

                // Call the service to search for messages
                var messages = await _messageService.SearchConversationAsync(query, currentUserId);

                return Ok(new { message = "Messages retrieved successfully", messages });
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, log the error, and return an error response
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }

    }
}
