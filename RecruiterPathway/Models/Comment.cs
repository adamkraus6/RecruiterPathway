using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class Comment
    {
        public Comment(string RecruiterId, DateTime Time, string ActualComment)
        {
            this.Id = Guid.NewGuid().ToString();
            this.RecruiterId = RecruiterId;
            this.Time = Time;
            this.ActualComment = ActualComment;
        }
        public string Id { get; set; }
        public string StudentId { get; set; }
        public Student Student { get; set; }
        public string RecruiterId { get; set; }
        public DateTime Time { get; set; }
        public string ActualComment { get; set; }
    }
}
