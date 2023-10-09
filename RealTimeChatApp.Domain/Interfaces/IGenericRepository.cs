using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Interfaces
{
    public interface IGenericRepository
    {
        Task AddMessageAsync(Message message);
        Task SaveChangesAsync();

        Task<Message> GetMessageByIdAsync(int messageId);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(Message message);
        //IQueryable<Message> GetConversationMessages(Guid senderId, Guid receiverId);
        // Task<Message> GetUserByIdAsync(Guid userId);

        //Task<User> GetUserByIdAsync(Guid userId);

    }
}
