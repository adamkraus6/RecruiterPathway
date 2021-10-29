using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RecruiterPathway.Models
{
    public class Recruiter : IdentityUser
    {
        public string Name { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Phone]
        [Display(Name = "Phone Number")]
        override public string PhoneNumber { get; set; }
        [MinLength(8)]
        [MaxLength(64)]
        public string Password { get; set; }
        [EmailAddress]
        override public string Email { get; set; }
        //Enables validation of the username as an email since we are requiring usernames to be emails.
        [EmailAddress]
        [Display(Name = "Email Address")]
        override public string UserName { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
