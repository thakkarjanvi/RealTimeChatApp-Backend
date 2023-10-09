using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.DAL.Context;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.DAL.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _dbContext;

        public LogService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Log> GetLogs(DateTime startTime, DateTime endTime)
        
        {
            return _dbContext.LogEntries
                         .Where(log => log.Timestamp >= startTime && log.Timestamp <= endTime)
                         .AsQueryable();
        }
    }
}
