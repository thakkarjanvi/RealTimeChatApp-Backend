using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Models
{
    public class SendMessage
    {
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
