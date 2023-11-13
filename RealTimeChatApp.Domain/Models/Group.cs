using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Models
{
    public class Group
    {
        [Required]
        public Guid Id { get; set; }

        public string GroupName { get; set; }

        // Navigation property for GroupMembers
        public List<GroupMember> Members { get; set; }

        // Navigation property for Messages
        public List<Message> Messages { get; set; }
    }
}
