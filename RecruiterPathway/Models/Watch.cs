using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class Watch
    {
        public string Id { get; set; }
        public virtual Student Student { get; set; }
        public virtual Recruiter Recruiter { get; set; }
    }
}
