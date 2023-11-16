using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task AddMessageAsync(Message message);
        Task SaveChangesAsync();

        Task<Message> GetMessageByIdAsync(int messageId);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(Message message);
        //IQueryable<Message> GetConversationMessages(Guid senderId, Guid receiverId);
        // Task<Message> GetUserByIdAsync(Guid userId);

        //Task<User> GetUserByIdAsync(Guid userId);

        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(int id);
        Task<TEntity> GetByIdAsync(int messageId);
        Task<List<TEntity>> GetAllAsync();
        Task<List<Group>> GetUserGroupsByUserIdAsync(string currentUserId);
        Task<GroupMember> MemberExistsInGroupAsync(Guid groupId, string memberId);
        Task<GroupMember> GetGroupMemberByIdAsync(string memberId);
        Task<bool> DeleteAsync(TEntity entity);
        Task<Group> GetGroupByIdAsync(Guid groupId);



    }
}
