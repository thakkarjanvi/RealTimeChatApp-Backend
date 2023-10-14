using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.DAL.Context;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.DAL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IGenericRepository _genericRepository;
        private readonly List<Message> _messages;
        private readonly ApplicationDbContext _dbContext;

        public MessageService(IGenericRepository genericRepository, List<Message> messages, ApplicationDbContext dbContext)
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

            // Create a new message entity
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = sendMessage.ReceiverId,
                Content = sendMessage.Content,
                Timestamp = DateTime.Now
            };

            // Add the message to the repository and save changes
            await _genericRepository.AddMessageAsync(message);
            await _genericRepository.SaveChangesAsync();

            // Map the entity to DTO and return
            var messageDto = new MessageDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                Timestamp = message.Timestamp
            };

            return messageDto;
        }


        //Edit Messsage
        public async Task<MessageDto> EditMessageAsync(int messageId, Guid senderId, EditMessage editMessage)
        {
            var message = await _genericRepository.GetMessageByIdAsync(messageId);

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
                ReceiverId = message.ReceiverId,
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
                ReceiverId = messageToDelete.ReceiverId,
                Content = messageToDelete.Content,
                Timestamp = messageToDelete.Timestamp
                // Include other properties as needed
            };
        }

        //Retrieve Conversation History
        public async Task<List<MessageDto>> RetrieveConversationHistory(ConversationHistoryDto queryParameters, Guid currentUserId)
        {

            var query = _dbContext.Messages.Where(m =>
                            (m.SenderId == currentUserId && m.ReceiverId == queryParameters.UserId) ||
                            (m.SenderId == queryParameters.UserId && m.ReceiverId == currentUserId))
                         .Where(m => m.Timestamp < queryParameters.Before).OrderByDescending(m => m.Timestamp)
        .Take(queryParameters.Count);



            //query = query.OrderByDescending(m => m.Timestamp);

            // Limit the number of messages retrieved based on the specified count
            //if (queryParameters.Count > 0)
            //{
            //    query = query.Take(queryParameters.Count);
            //}

            // Sort the messages based on the specified sort order
            //if (queryParameters.SortOrder == SortOrder.Ascending)
            //{
            //    query = query.OrderBy(m => m.Timestamp);
            //}
            //else
            //{
            //    query = query.OrderByDescending(m => m.Timestamp);
            //}

            // Execute the query and return the conversation history
            var conversationMessages = query.ToList();

            // Map messages to MessageDto objects
            var messageDtos = conversationMessages.Select(m => new MessageDto
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                Timestamp = m.Timestamp
            }).ToList();

            return messageDtos;
        }

        public async Task<List<MessageDto>> SearchConversationAsync(string query, Guid currentUserId)
        {
            var messages = await _dbContext.Messages
                .Where(m => (m.SenderId == currentUserId || m.ReceiverId == currentUserId) && m.Content.Contains(query))
                .ToListAsync();

            var messageDtos = messages.Select(message => new MessageDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                Timestamp = message.Timestamp
            }).ToList();

            return messageDtos;
        }
    

    }
}

