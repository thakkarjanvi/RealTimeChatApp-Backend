using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RealTimeChatApp.Domain.DTO
{
    public class ConversationHistoryDto
    {
        public Guid UserId { get; set; }
        public DateTime Before { get; set; } = DateTime.Now;
        public int Count { get; set; } = 20;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
    }
}
