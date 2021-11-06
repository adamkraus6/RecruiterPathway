using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class Watch
    {
        public Watch() { }
        public Watch(Student student, Recruiter recruiter)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Student = student;
            this.Recruiter = recruiter;
        }
        public string Id { get; set; }
        public virtual Student Student { get; set; }
        public virtual Recruiter Recruiter { get; set; }
    }
}
