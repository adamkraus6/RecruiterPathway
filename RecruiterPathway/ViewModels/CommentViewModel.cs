using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.ViewModels
{
    public class CommentViewModel
    {
        public Recruiter Recruiter { get; set; }
        public Student Student { get; set; }
        public string Comment { get; set; }
    }
}
