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

        public int? ParentProjectId { get; set; }
        public Project ParentProject { get; set; }

        public List<SubTask> SubTasks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}