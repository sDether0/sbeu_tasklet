using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SBEU.Exceptions;
using SBEU.Tasklet.DataLayer.DataBase.Entities;

namespace SBEU.Tasklet.DataLayer.DataBase
{
    public class ApiDbContext : IdentityDbContext<XIdentityUser>
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) {
            //Database.Migrate();
        }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<XIdentityUserConfirm> UserConfirmations { get; set; }
        public virtual DbSet<XTable> XTables { get; set; }
        public virtual DbSet<XTask> XTasks { get; set; }

        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<XMessage> Messages { get; set; }
        public virtual DbSet<XContent> Contents { get; set; }
        public virtual DbSet<XHistory> History { get; set; }
        public virtual DbSet<TaskProgress> TaskProgress { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<XIdentityUser>().HasMany(x => x.Tasks).WithOne(x => x.Executor);
            builder.Entity<XIdentityUser>().HasMany(x => x.AuthoredTasks).WithOne(x => x.Author);
            builder.Entity<Note>().HasKey(x => new { x.UserId, x.TaskId });
            builder.Entity<Note>().HasIndex(x=>x.UserId);
            builder.Entity<XHistory>().Property(x=>x.Id).ValueGeneratedOnAdd();
            builder.Entity<XTask>().HasMany(x => x.Contents).WithMany();
            builder.Entity<XTask>().HasOne(x=>x.Status).WithMany();
            builder.Entity<XHistory>().HasMany(x => x.Contents).WithMany();
            builder.Entity<XIdentityUser>().HasMany(x=>x.Notes).WithOne().HasForeignKey(x=>x.UserId);
            builder.Entity<XTask>().HasMany(x => x.Notes).WithOne().HasForeignKey(x => x.TaskId);
            builder.Entity<XTask>().Property(x => x.StartTime).HasConversion(to=>to.ToUniversalTime(),from=>from.ToUniversalTime());
            builder.Entity<XMessage>().Property(x => x.Time).HasConversion(to => to.ToUniversalTime(), from => from.ToUniversalTime());
            builder.Entity<XHistory>().Property(x => x.UpdateDate).HasConversion(to => to.ToUniversalTime(), from => from.ToUniversalTime());
            base.OnModelCreating(builder);
        }
        
    }

    public static class Extensions
    {
        public static T Get<T>(this string Id, ApiDbContext context) where T : class
        {
           return context.Find<T>(Id)??throw new EntityNotFoundException($"Entity {typeof(T)} not found");
        }

        public static TaskProgress GetProgress(this string status, ApiDbContext context)
        {
            return context.TaskProgress.FirstOrDefault(x => x.Status == status)??throw new EntityNotFoundException("Task status not found");
        }
    }

}
