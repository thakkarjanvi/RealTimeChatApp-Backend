using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.DTO
{
    public class GroupMemberDto
    {
        public Guid GroupId { get; set; }
        public string UserId { get; set; }
        public bool IsAdmin { get; set; }
        public string UserName { get; set; }
    }
}
