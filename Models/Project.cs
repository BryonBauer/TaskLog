using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskLog.Models;

namespace TaskLog.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required(ErrorMessage="Project Name is required")]
        [Display(Name="Project Name")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage="Project Description is required")]
        [Display(Name="Project Description")]
        public string ProjectDescription { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name="Due Date")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage="Project Status is required")]
        [Display(Name="Project Status")]
        public string ProjectStatus { get; set; }

        [Required(ErrorMessage="Estimated Time is required")]
        [Display(Name="Estimated Time")]   
        public int EstimatedTime { get; set; }

        // Simple way of checking for tasks
        public bool hasTask { get; set; }

        // User information
        public int? ProjectCreatorUserId { get; set; }
        public User ProjectCreator { get; set; }

        // Project's tasks
        public List<Task> ProjectTasks = new List<Task> ();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}