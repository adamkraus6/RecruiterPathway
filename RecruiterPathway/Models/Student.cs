using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruiterPathway.Models
{
    public class Student : IEquatable<Student>
    {
        public string Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Degree")]
        public string Degree { get; set; }
        
        [Display(Name = "Graduation Date")]
        [DataType(DataType.Date)]
        public DateTime gradDate { get; set; }

        //std::list<Triple<string, DateTime, string>>
        //List of Recruiter Ids to timestamped comments. These are global to all recruiters.
        public virtual ICollection<Comment> Comments { get; set; }

        public bool Equals(Student other)
        {
            return Id.Equals(other.Id) && FirstName.Equals(other.FirstName) && LastName.Equals(other.LastName) && Degree.Equals(other.Degree) && gradDate == other.gradDate;
        }
    }
}
