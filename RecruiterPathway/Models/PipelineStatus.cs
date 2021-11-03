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
            this.StudentId = StudentId;
            this.Status = Status;
        }
        public string Id { get; set; }
        public string RecruiterId { get; set; }
        public Recruiter Recruiter { get; set; }
        public string StudentId { get; set; }
        public string Status { get; set; }
    }
}
