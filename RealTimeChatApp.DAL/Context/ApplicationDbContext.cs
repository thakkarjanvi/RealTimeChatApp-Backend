using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.DAL.Context
{
    public class ApplicationDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Log> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Message and Thread using Fluent API
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Thread) // 
                .WithMany(t => t.Messages) 
                .HasForeignKey(m => m.ThreadId) 
                .IsRequired(false);

            // Primary key configuration (assuming MessageId is primary key for Message entity)
            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageId);
        }
    }
}
