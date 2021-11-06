using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class PipelineStatus
    {
        public PipelineStatus(string StudentId, string Status)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Status = Status;
        }
        public string Id { get; set; }
        public virtual Recruiter Recruiter { get; set; }
        public virtual Student Student { get; set; }
        public string Status { get; set; }
    }
}
