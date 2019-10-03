using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskLog.Models;

namespace TaskLog.Models
{
    public class SubTask
    {
        [Key]
        public int SubTaskID { get; set; }

        [Required(ErrorMessage="Sub Task Name is required")]
        [Display(Name="Sub Task Name")]
        public string SubTaskName { get; set; }

        [Required(ErrorMessage="Sub Task Description is required")]
        [Display(Name="Sub Task Description")]
        public string SubTaskDescription { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name="Due Date")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage="Estimated Time is required")]
        [Display(Name="Estimated Time")]
        public int EstimatedTime { get; set; }

        public int? ParentTaskId { get; set; }
        public Task ParentTask { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}