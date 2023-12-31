﻿using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Models;
using RealTimeChatApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto> SendMessageAsync(Guid senderId, SendMessage sendMessage);
        Task<MessageDto> EditMessageAsync(int messageId, Guid senderId, EditMessage editMessage);

        Task<MessageDto> DeleteMessageAsync(int messageId, Guid senderId);
        Task<List<Message>> GetConversationHistoryAsync(ConversationHistoryDto queryParameters, Guid currentUserId);
        Task<IEnumerable<MessageDto>> SearchConversationAsync(string query, Guid currentUserId);
        Task<List<Message?>> GetThreadMessagesAsync(int threadId);

    }
}