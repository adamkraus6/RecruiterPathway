using Microsoft.AspNetCore.Identity;
namespace TeamProject.Authentication
{
    public class AuthLevels : IdentityRole
    {
        public const string USER = "0";
        public const string ADMIN = "1";
    }
}
