using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class Comment
    {
        public Comment() { }
        public Comment(Recruiter Recruiter, Student Student, DateTime Time, string ActualComment)
        {
            this.Recruiter = Recruiter;
            this.Student = Student;
            this.Id = Guid.NewGuid().ToString();
            this.Time = Time;
            this.ActualComment = ActualComment;
        }
        public Comment(Recruiter Recruiter, Student Student, string ActualComment)
        {
            this.Recruiter = Recruiter;
            this.Student = Student;
            this.Id = Guid.NewGuid().ToString();
            this.Time = DateTime.Now;
            this.ActualComment = ActualComment;
        }
        public string Id { get; set; }
        public virtual Student Student { get; set; }
        public virtual Recruiter Recruiter { get; set; }
        [DataType(DataType.Date)]
        public DateTime Time { get; set; }
        public string ActualComment { get; set; }
    }
}
