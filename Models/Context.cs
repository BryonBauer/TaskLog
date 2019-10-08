using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskLog.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) { }
        
        // All the Users, Projects, Tasks, and SubTasks
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // On creation of Project make the ProjectID the unique index
            modelBuilder.Entity<Project>()
                        .HasIndex(p => new { p.ProjectID})
                        .IsUnique(true);

            // One Project can have many tasks and many tasks can have one parent project
            modelBuilder.Entity<Project>()
                        .HasOne(p => p.ProjectCreator)
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade); 

            // One Task can have many tasks and many tasks can have one parent Task
            modelBuilder.Entity<Task>()
                        .HasOne(p => p.ParentProject)
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade); 

            // One Task can have many subtasks and many subtasks can have one parent task
            modelBuilder.Entity<SubTask>()
                        .HasOne(p => p.ParentTask)
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}