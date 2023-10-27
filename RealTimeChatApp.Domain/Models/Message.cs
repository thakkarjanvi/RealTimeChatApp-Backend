using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int? ThreadId { get; set; }

        // Navigation property for self-referencing relationship
        public Message? Thread { get; set; }

        // One-to-many relationship with child messages
        public List<Message> Messages { get; set; }

    }
}
