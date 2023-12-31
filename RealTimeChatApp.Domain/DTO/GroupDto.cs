﻿using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.DTO
{
    public class GroupDto
    {
       
        public string GroupName { get; set; }

        // Navigation property for GroupMembers
        public List<Guid>? Members { get; set; }
    }
}
