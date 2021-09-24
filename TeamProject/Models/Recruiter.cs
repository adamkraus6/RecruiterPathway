using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TeamProject.Models
{
    public class Recruiter
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        //This has to be called username to allow authentication to work. 
        //It is also the user's email
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
