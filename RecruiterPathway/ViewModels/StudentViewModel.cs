using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public SelectList SortOptions { get; set; }
        public string SortBy { get; set; }

        public Comment Comment;
        public string AddCommentText { get; set; }

        public SelectList Statuses { get; set; }
        public string PipelineStatus { get; set; }
        public string NewPipelineStatus { get; set; }
    }
}
