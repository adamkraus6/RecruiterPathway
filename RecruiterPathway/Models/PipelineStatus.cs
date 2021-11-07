using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class PipelineStatus
    {
        public PipelineStatus() { }
        public PipelineStatus(Student student, Recruiter recruiter, string status)
        {
            this.Student = student;
            this.Recruiter = recruiter;
            this.Id = Guid.NewGuid().ToString();
            this.Status = status;
        }
        public string Id { get; set; }
        public virtual Recruiter Recruiter { get; set; }
        public virtual Student Student { get; set; }
        public string Status { get; set; }
    }
}
