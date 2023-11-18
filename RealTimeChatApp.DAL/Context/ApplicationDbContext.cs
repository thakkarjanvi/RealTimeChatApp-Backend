using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.RegularExpressions;
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

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Message and Thread using Fluent API
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Thread) // 
                .WithMany(t => t.Messages) 
                .HasForeignKey(m => m.ThreadId) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Primary key configuration (assuming MessageId is primary key for Message entity)
            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageId);

            modelBuilder.Entity<Message>()
                   .Property(m => m.SenderId).IsRequired(true);
            modelBuilder.Entity<Message>()
                   .Property(m => m.ReceiverId).IsRequired(false);
            modelBuilder.Entity<Message>()
                  .Property(m => m.Content).IsRequired(false);
            modelBuilder.Entity<Message>()
                  .Property(m => m.Timestamp).IsRequired(true);
            modelBuilder.Entity<Message>()
                  .Property(m => m.GroupId).IsRequired(false);


            modelBuilder.Entity<GroupMember>()
                 .HasKey(gu => new { gu.UserId, gu.GroupId });

            modelBuilder.Entity<GroupMember>()
                .HasOne(u => u.User)
                .WithMany(gu => gu.GroupMembers)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(g => g.Group)
                .WithMany(gu => gu.Members)
                .HasForeignKey(g => g.GroupId);

            modelBuilder.Entity<Message>()
               .HasOne(m => m.Group)
               .WithMany(g => g.Messages)
               .HasForeignKey(m => m.GroupId);

        }
    }
}
