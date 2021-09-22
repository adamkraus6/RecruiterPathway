using System;
using System.ComponentModel.DataAnnotations;

namespace TeamProject.Models
{
    public class Recruiter
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
