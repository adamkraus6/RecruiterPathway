using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TeamProject.Models
{
    public class Recruiter : IdentityUser
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }

        public string Password { get; set; }
    }
}
