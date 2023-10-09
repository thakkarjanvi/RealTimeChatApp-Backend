using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.Domain.Interfaces
{
    public interface ILogService
    {
        public IQueryable<Log> GetLogs(DateTime startTime, DateTime endTime);
    }
}
