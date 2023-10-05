﻿using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(Register register);
    }
}
