using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class StudentDegreeViewModel
    {
        public List<Student> Students { get; set; }
        public SelectList Degrees { get; set; }
        public string StudentDegree { get; set; }
        public string SearchFirstName { get; set; }
        public string SearchLastName { get; set; }
        public DateTime GradDateStart { get; set; }
        public DateTime GradDateEnd { get; set; }
    }
}
