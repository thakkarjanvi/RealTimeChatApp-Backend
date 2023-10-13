using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.DTO
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string JwtToken { get; set; }
        public UserDto Profile { get; set; }
    }
}
