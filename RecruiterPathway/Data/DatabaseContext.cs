using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Models;

namespace RecruiterPathway.Data
{
    public class DatabaseContext : IdentityDbContext<Recruiter>
    { 
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){

        }
        public DbSet<Recruiter> Recruiter { get; set; }
        public DbSet<Student> Student { get; set; }
    }
}
