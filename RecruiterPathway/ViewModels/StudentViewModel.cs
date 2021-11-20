using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruiterPathway.ViewModels
{
    public class StudentViewModel
    {
        public List<Student> Students { get; set; }
        public Student Student { get; set; }

        public SelectList Degrees { get; set; }
        public string StudentDegree { get; set; }
        public string SearchFirstName { get; set; }
        public string SearchLastName { get; set; }
        public DateTime GradDateStart { get; set; }
        public DateTime GradDateEnd { get; set; }

        public bool ListView { get; set; }
        public SortOptions SortBy { get; set; }

        public Comment Comment;
        public string AddCommentText { get; set; }

        public SelectList Statuses { get; set; }
        public string PipelineStatus { get; set; }
        public string NewPipelineStatus { get; set; }

        public enum SortOptions
        {
            [Display(Name = "None")]
            NONE,
            [Display(Name = "First Name")]
            FIRST,
            [Display(Name = "Last Name")]
            LAST,
            [Display(Name = "Degree")]
            DEG,
            [Display(Name = "Graduation Date")]
            GRAD
        }
    }
}
