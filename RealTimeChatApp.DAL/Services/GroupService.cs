using RealTimeChatApp.Domain.DTO;
using AutoMapper;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealTimeChatApp.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace RealTimeChatApp.DAL.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGenericRepository<Group> _groupRepository;
        private readonly IGenericRepository<GroupMember> _groupMemberRepository;
        private readonly ApplicationDbContext _dbContext;
        public GroupService(IGenericRepository<Group> groupRepository, IGenericRepository<GroupMember> groupMemberRepository, ApplicationDbContext dbContext)
        {
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _dbContext = dbContext;
           
        }

        //Create Group
        // Create Group
        public async Task<ResponseGroupDto> CreateGroupAsync(string? currentUser, GroupDto groupDto)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                GroupName = groupDto.GroupName,
            };

            Guid currentUserGuid = Guid.Parse(currentUser!);

            if (!groupDto.Members!.Contains(currentUserGuid))
            {
                groupDto.Members.Add(currentUserGuid);
            }

            var addedGroup = await _groupRepository.AddAsync(group);

            if (groupDto.Members != null)
            {
                foreach (var memberId in groupDto.Members)
                {
                    var isAdmin = memberId.ToString() == currentUserGuid.ToString();
                    var groupMember = new GroupMember
                    {
                        GroupId = addedGroup.Id,
                        UserId = memberId.ToString(),
                        IsAdmin = isAdmin,
                    };

                    await _groupMemberRepository.AddAsync(groupMember);
                }
            }

            // Manually create ResponseGroupDto without AutoMapper
            var responseGroupDto = new ResponseGroupDto
            {
                Id = addedGroup.Id,
                GroupName = addedGroup.GroupName,
                // Add other properties as needed
            };

            return responseGroupDto;
        }


        public async Task<string> AddMemberToGroupAsync(Guid groupId, string? currentUserId, AddGroupMemberDto addGroupMemberDto)
        {
            // Check if the group exists
            var group = await _dbContext.Groups.FirstOrDefaultAsync(grp => grp.Id == groupId);
            if (group == null)
            {
                return "Group not found";
            }

            var groupMember = await _dbContext.GroupMembers.FirstOrDefaultAsync(grpmem => grpmem.UserId == currentUserId.ToString());

            if (groupMember != null && !groupMember.IsAdmin)
            {
                return "You are not an admin! You can't add members!";
            }


            if (addGroupMemberDto.memberId != Guid.Empty)
            {
                var memberId = addGroupMemberDto.memberId;

                // Check if the member already exists in the group
                var memberExists = await _dbContext.GroupMembers.AnyAsync(grpmem => grpmem.GroupId == groupId && grpmem.UserId == memberId.ToString());

                if (memberExists)
                {
                    // Handle the case where the member already exists in the group
                    return $"Member with ID {memberId} already exists in the group";
                }

                var isAdmin = memberId == Guid.Parse(currentUserId!);
                var groupUser = new GroupMember
                {
                    GroupId = groupId,
                    UserId = memberId.ToString(),
                    IsAdmin = isAdmin,
                };

                await _groupMemberRepository.AddAsync(groupUser);
                return "Member Added Successfully!";
            }
            else
            {
                return "No member ID provided.";
            }
        }

        //Remove Member

        public async Task<string> RemoveMemberFromGroupAsync(Guid groupId, string? currentUserId, Guid memberId)
        {
            // Check if the group exists
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                return "Group not found";
            }

            var groupMember = await _groupMemberRepository.MemberExistsInGroupAsync(groupId, currentUserId);

            if (groupMember != null && !groupMember.IsAdmin)
            {
                return "You are not an admin! You can't remove members!";
            }

            // Check if the member exists in the group
            var memberExists = await _groupMemberRepository.MemberExistsInGroupAsync(groupId, memberId.ToString());

            if (memberExists == null)
            {
                return $"Member with ID {memberId} not found in the group";
            }

            // Remove the member from the group
            await _groupMemberRepository.DeleteAsync(memberExists);

            return "Member Removed Successfully!";
        }

        //Edit Group Name
        public async Task<string> EditGroupNameAsync(Guid groupId, string newName)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
            {
                return "Group not found";
            }

            group.GroupName = newName;
            await _groupRepository.UpdateAsync(group);

            return "Name updated sucessfully!";
        }

        //Make admin

        public async Task<string> MakeMemberAdminAsync(Guid groupId, Guid memberId, string? currentUserId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
            {
                return "Group not found";
            }
            var currentUser = await _groupMemberRepository.MemberExistsInGroupAsync(groupId, currentUserId);

            if (currentUser != null && !currentUser.IsAdmin)
            {
                return "You are not an admin! Member can't make admin to anyone!";
            }

            var groupMember = await _groupMemberRepository.MemberExistsInGroupAsync(groupId, memberId.ToString());

            if (groupMember == null)
            {
                return "Member not found in the group";
            }

            groupMember.IsAdmin = true;

            await _groupMemberRepository.UpdateAsync(groupMember);

            return "Now you are an admin";
        }

        //Delete Group
        public async Task<string> DeleteGroupAsync(Guid groupId, string? currentUser)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
            {
                return "Group not found";
            }

            var groupMember = await _groupMemberRepository.MemberExistsInGroupAsync(groupId, currentUser);

            if (groupMember == null || !groupMember.IsAdmin)
            {
                return "You do not have permission to delete this group";
            }

            await _groupRepository.DeleteAsync(group);

            return "Group deleted successfully";
        }

    }
}
