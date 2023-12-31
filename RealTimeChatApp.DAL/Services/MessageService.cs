﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.DAL.Context;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RealTimeChatApp.DAL.Repository;

namespace RealTimeChatApp.DAL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IGenericRepository<Message> _genericRepository;
        private readonly List<Message> _messages;
        private readonly ApplicationDbContext _dbContext;


        public MessageService(IGenericRepository<Message> genericRepository, List<Message> messages, ApplicationDbContext dbContext)
        {
            _genericRepository = genericRepository;
            _messages = messages;
            _dbContext = dbContext;

        }


        // Send Message
        public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessage sendMessage)
        {
            // Validate the incoming message
            if (sendMessage == null || sendMessage.ReceiverId == Guid.Empty || string.IsNullOrWhiteSpace(sendMessage.Content))
            {
                throw new ArgumentException("Invalid message data.");
            }
            Message message;
            if (sendMessage.ThreadId == null)
            {
                message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = sendMessage.ReceiverId,
                    Content = sendMessage.Content,
                    Timestamp = DateTime.Now,
                };
            }
            else
            {
                message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = sendMessage.ReceiverId,
                    Content = sendMessage.Content,
                    Timestamp = DateTime.Now,
                    ThreadId = sendMessage.ThreadId,
                };
            }

            // Add the message to the repository and save changes
            await _genericRepository.AddMessageAsync(message);
            await _genericRepository.SaveChangesAsync();

            // Map the entity to DTO and return
            var messageDto = new MessageDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId ?? Guid.Empty,
                Content = message.Content,
                Timestamp = message.Timestamp
            };

            return messageDto;
        }


        //Edit Messsage
        public async Task<MessageDto> EditMessageAsync(int messageId, Guid senderId, EditMessage editMessage)
        {
            var message = await _genericRepository.GetByIdAsync(messageId);

            if (message == null)
            {
                return null; // Message not found, handle this appropriately in the controller
            }


            if (message.SenderId != senderId)
            {
                return null; // Unauthorized, handle this appropriately in the controller
            }

            message.Content = editMessage.Content;
            message.Timestamp = DateTime.Now;

            await _genericRepository.UpdateMessageAsync(message);

            var messageDto = new MessageDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId ?? Guid.Empty,
                Content = message.Content,
                Timestamp = message.Timestamp
            };

            return messageDto;
        }

        //Delete Message
        public async Task<MessageDto> DeleteMessageAsync(int messageId, Guid senderId)
        {
            var messageToDelete = await _genericRepository.GetMessageByIdAsync(messageId);

            if (messageToDelete == null)
            {
                return null;
            }

            if (messageToDelete.SenderId != senderId)
            {
                return null;
            }

            await _genericRepository.DeleteMessageAsync(messageToDelete);
            return new MessageDto
            {
                MessageId = messageToDelete.MessageId,
                SenderId = messageToDelete.SenderId,
                ReceiverId = messageToDelete.ReceiverId ?? Guid.Empty,
                Content = messageToDelete.Content,
                Timestamp = messageToDelete.Timestamp
                // Include other properties as needed
            };
        }

        //Retrieve Conversation History
        public async Task<List<Message>> GetConversationHistoryAsync(ConversationHistoryDto queryParameters, Guid currentUserId)
        {
            var query = _dbContext.Messages
                 .Where(m => (m.ThreadId == null) &&
                    (m.SenderId == currentUserId && m.ReceiverId == queryParameters.UserId) ||
                    (m.SenderId == queryParameters.UserId && m.ReceiverId == currentUserId))
                .Where(m => m.Timestamp <= queryParameters.Before);


            Console.WriteLine("Query", query);

            if (queryParameters.SortOrder.Equals(SortOrder.Ascending))
            {
                query = query.OrderBy(m => m.Timestamp);
            }

            if (queryParameters.Count > 0)
            {
                query = query.Take(queryParameters.Count);
            }

            var messages = query.ToList();

            return messages;
        }

        // Search Conversation
        public async Task<IEnumerable<MessageDto>> SearchConversationAsync(string query, Guid currentUserId)
        {
            var messages = _dbContext.Messages
               .Where(m => (m.SenderId == currentUserId || m.ReceiverId == currentUserId) && m.Content.Contains(query))
               .ToList();

            var messageDtos = messages.Select(message => new MessageDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId ?? Guid.Empty,
                Content = message.Content,
                Timestamp = message.Timestamp
            }).ToList();

            return messageDtos;
        }

        public async Task<List<Message?>> GetThreadMessagesAsync(int threadId)
        {
            return _dbContext.Messages.Where(m => m.ThreadId == threadId).ToList();
        }
    }
}
