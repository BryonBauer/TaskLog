using Microsoft.EntityFrameworkCore; 

namespace TaskLog.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One Project can have many tasks and many tasks can have one parent project
            modelBuilder.Entity<Task>()
                        .HasOne(b => b.ParentProject)
                        .WithMany(c => c.Tasks)
                        .OnDelete(DeleteBehavior.Cascade);

            // One Task can have many subtasks and many subtasks can have one parent task
            modelBuilder.Entity<SubTask>()
                        .HasOne(a => a.ParentTask)
                        .WithMany(b => b.SubTasks)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}