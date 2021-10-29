using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Models;

namespace RecruiterPathway.Data
{
    public class DatabaseContext : IdentityDbContext<Recruiter>
    { 
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){

        }
        public DatabaseContext() { }
        public virtual DbSet<Recruiter> Recruiter { get; set; }
        public virtual DbSet<Student> Student { get; set; }
    }
}
