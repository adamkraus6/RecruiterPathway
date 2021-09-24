using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamProject.Models;

namespace TeamProject.Data
{
    public class AuthenticationDbContext : IdentityDbContext<Recruiter>
    { 
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options){

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Recruiter> Recruiter { get; set; }
    }
}
