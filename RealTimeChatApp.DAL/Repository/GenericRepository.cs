using RealTimeChatApp.DAL.Context;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.DAL.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == messageId);

        }

        public async Task UpdateMessageAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(Message message)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        //public IQueryable<Message> GetConversationMessages(Guid senderId, Guid receiverId)
        //{
        //    return _context.Messages
        //        .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
        //                    (m.SenderId == receiverId && m.ReceiverId == senderId));
        //}

        //public async Task<User> GetUserByIdAsync(Guid userId)
        //{
        //    Convert Guid to string for comparison with Id in the database
        //    string userIdString = userId.ToString();

        //    return await _context.Users.FirstOrDefaultAsync(u => u.Id == userIdString);
        //}

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
        public async Task<List<Group>> GetUserGroupsByUserIdAsync(string currentUserId)
        {
            return await _context.Groups.Include(x => x.Members).Where(g => g.Members.Any(m => m.UserId == currentUserId)).ToListAsync();
        }

        public async Task<GroupMember> MemberExistsInGroupAsync(Guid groupId, string memberId)
        {
            return await _context.Set<GroupMember>().FirstOrDefaultAsync(grpmem => grpmem.GroupId == groupId && grpmem.UserId == memberId);
        }

        public async Task<GroupMember> GetGroupMemberByIdAsync(string memberId)
        {
            return await _context.Set<GroupMember>().FirstOrDefaultAsync(grpmem => grpmem.UserId == memberId);
        }

        public async Task<Group> GetGroupByIdAsync(Guid groupId)
        {
            return await _context.Set<Group>().FirstOrDefaultAsync(grp => grp.Id == groupId);
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}

