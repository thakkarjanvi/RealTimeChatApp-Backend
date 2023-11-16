using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.DTO
{
    public class GetMessageDto
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }


        public string? ReceiverId { get; set; }
        public Guid? GroupId { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        public List<GroupMemberDto?> Users { get; set; }
    }
}
