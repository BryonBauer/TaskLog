using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskLog.Models;

namespace TaskLog.Models
{
    public class Task
    {
        [Key]
        public int TaskID { get; set; }

        [Required(ErrorMessage="Task Name is required")]
        [Display(Name="Task Name")]
        public string TaskName { get; set; }

        [Required(ErrorMessage="Task Description is required")]
        [Display(Name="Task Description")]
        public string TaskDescription { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name="Due Date")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage="Estimated Time is required")]
        [Display(Name="Estimated Time")]
        public int EstimatedTime { get; set; }

        //Simple way of checking for SubTasks
        public bool hasSubTasks { get; set; }

        // Project Info
        public int? ParentProjectId { get; set; }
        public Project ParentProject { get; set; }

        // User info
        public int? TaskCreatorUserId { get; set; }
        public User TaskCreator { get; set; }

        // List of Tasks SubTasks
        public List<SubTask> TaskSubTasks = new List<SubTask> ();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}