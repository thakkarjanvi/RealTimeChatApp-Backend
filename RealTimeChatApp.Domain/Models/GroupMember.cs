using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Models
{
    public class GroupMember
    {
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public bool IsAdmin { get; set; }

    }
}
