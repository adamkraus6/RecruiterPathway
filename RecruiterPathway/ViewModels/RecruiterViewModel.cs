using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.ViewModels
{
    public class RecruiterViewModel
    {
        public List<Recruiter> Recruiters { get; set; }
        public Recruiter Recruiter { get; set; }
    }
}
