using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamProject.Models;

namespace TeamProject.Authentication
{
    public class AuthenticationDbContext : IdentityDbContext<AuthUser>
    { 
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options){

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
