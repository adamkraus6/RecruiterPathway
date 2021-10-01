using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TeamProject.Models
{
    public class Recruiter : IdentityUser
    {
        public string Name { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [MinLength(8)]
        [MaxLength(64)]
        public string Password { get; set; }
        [EmailAddress]
        override public string Email { get; set; }
    }
}
