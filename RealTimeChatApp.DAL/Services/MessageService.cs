using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.DAL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IGenericRepository _genericRepository;

        public MessageService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

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


    }
}
