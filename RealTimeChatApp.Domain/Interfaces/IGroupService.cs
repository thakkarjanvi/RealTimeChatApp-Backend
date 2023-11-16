using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Interfaces
{
    public interface IGroupService
    {
        Task<ResponseGroupDto> CreateGroupAsync(string? currentUserId, GroupDto groupDto);
        Task<string> AddMemberToGroupAsync(Guid groupId, string? currentUserId, AddGroupMemberDto addGroupMemberDto);
        Task<string> RemoveMemberFromGroupAsync(Guid groupId, string? currentUserId, Guid memberId);
        Task<string> EditGroupNameAsync(Guid groupId, string newName);
        Task<string> MakeMemberAdminAsync(Guid groupId, Guid memberId, string? currentUserId);
        Task<string> DeleteGroupAsync(Guid groupId, string? currentUser);

    }
}
