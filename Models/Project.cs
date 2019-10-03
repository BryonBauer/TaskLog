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
        public string ProjectStatus = "New";

        [Required(ErrorMessage="Estimated Time is required")]
        [Display(Name="Estimated Time")]   
        public int EstimatedTime { get; set; }

        public bool hasTask { get; set; }

        public User ProjectCreator { get; set; }

        public int UserId { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}