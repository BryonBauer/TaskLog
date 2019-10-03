using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

            // // One User can have many projects and many projects can have one user
            // modelBuilder.Entity<Project>()
            //             .HasOne(p => p.ProjectCreator)
            //             .WithMany(p => p.UserProjects)
            //             .OnDelete(DeleteBehavior.Restrict); 

            // // One Project can have many tasks and many tasks can have one parent project
            // modelBuilder.Entity<Task>()
            //             .HasOne(p => p.TaskCreator)
            //             .WithMany(p => p.UserTasks)
            //             .OnDelete(DeleteBehavior.Restrict); 

            // // One Task can have many subtasks and many subtasks can have one parent task
            // modelBuilder.Entity<SubTask>()
            //             .HasOne(p => p.SubTaskCreator)
            //             .WithMany(p => p.UserSubTasks)
            //             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}