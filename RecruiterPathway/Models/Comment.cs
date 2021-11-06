using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class Comment
    {
        public Comment() { }
        public Comment(string RecruiterId, string StudentId, DateTime Time, string ActualComment)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Time = Time;
            this.ActualComment = ActualComment;
        }
        public Comment(string RecruiterId, string StudentId, string ActualComment)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Time = DateTime.UtcNow;
            this.ActualComment = ActualComment;
        }
        public string Id { get; set; }
        public virtual Student Student { get; set; }
        public virtual Recruiter Recruiter { get; set; }
        public DateTime Time { get; set; }
        public string ActualComment { get; set; }
    }
}
