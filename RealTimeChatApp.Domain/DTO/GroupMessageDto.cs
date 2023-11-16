using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.DTO
{
    public class GroupMessageDto
    {
        public List<ResponseMessageDto?> Messages { get; set; }
        public List<GroupMemberDto?> Members { get; set; }
    }
}
