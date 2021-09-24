using System.ComponentModel.DataAnnotations;

namespace TeamProject.Authentication
{
    public class LoginModel
    {
        /*
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "CompanyName is required")]
        public string CompanyName { get; set; }
        */
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }
}
