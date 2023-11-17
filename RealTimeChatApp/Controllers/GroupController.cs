using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApp.DAL.Services;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System.Net;
using System.Security.Claims;

namespace RealTimeChatApp.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        //Create Group

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroupAsync([FromBody] GroupDto groupDto)
        {
            try
            {
                // Check model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Invalid model data" });
                }

                // Get the current user's ID from the claims
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var addedGroup = await _groupService.CreateGroupAsync(currentUserId, groupDto);
                return Ok(new {Message = "Group created successfully"});
            }

            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        //Retrive User List

        
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var groupDtos = await _groupService.GetAllGroupsAsync();

            return Ok(groupDtos);
        }

        //Addmember

        [HttpPost("{groupId}/add-member")]
        public async Task<IActionResult> AddMemberToGroupAsync(Guid groupId, [FromBody] AddGroupMemberDto addGroupMemberDto)
        {
            try
            {
                // Get the current user's ID from the claims
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var result = await _groupService.AddMemberToGroupAsync(groupId, currentUserId, addGroupMemberDto);

                return Ok(new {Message = "Member Added Successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        //Remove Member

        [HttpPost("{groupId}/remove-member")]
        public async Task<IActionResult> RemoveMemberFromGroupAsync(Guid groupId, [FromQuery] Guid memberId)
        {
            try
            {
                // Get the current user's ID from the claims
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var result = await _groupService.RemoveMemberFromGroupAsync(groupId, currentUserId, memberId);

                return Ok(new { Message = "Member removed successfully" });
                
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        //Edit Group Name

        [HttpPut("{groupId}/edit-group-name")]
        public async Task<IActionResult> EditGroupNameAsync(Guid groupId, [FromQuery] string newName)
        {
            try
            {
                var updatedGroupResult = await _groupService.EditGroupNameAsync(groupId, newName);

                return Ok(new { Message = "Edit Groupname Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        //Make-Admin

        [HttpPut("make-member-admin")]
        public async Task<IActionResult> MakeMemberAdminAsync([FromQuery] Guid groupId, [FromQuery] Guid memberId)
        {
            try
            {
                // Get the current user's ID from the claims
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var result = await _groupService.MakeMemberAdminAsync(groupId, memberId, currentUserId);

                return Ok(new { Message = "Member is now an Admin" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        //Delete Group

        [HttpDelete("{groupId}/delete-group")]
        public async Task<IActionResult> DeleteGroupAsync(Guid groupId)
        {
            try
            {
                // Get the current user's ID from the claims
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var deleteGroupResult = await _groupService.DeleteGroupAsync(groupId, currentUserId);

                return Ok(new { Message = "Delete Group Successfully" });
                
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }



    }
}
