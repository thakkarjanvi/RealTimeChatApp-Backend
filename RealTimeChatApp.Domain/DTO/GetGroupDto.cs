using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.DTO
{
    public class GetGroupDto
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }

        // Navigation property for GroupMembers

        public ICollection<GroupMember>? GroupMembers { get; set; } // Use List instead of ICollection

        public string CreatorUserId { get; set; }

        // Navigation property for Messages
        public List<Message>? Messages { get; set; }
    }
}
