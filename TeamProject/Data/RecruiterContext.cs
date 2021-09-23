using Microsoft.EntityFrameworkCore;
using TeamProject.Models;

namespace TeamProject.Data
{
    public class RecruiterContext : DbContext
    {
        public RecruiterContext(DbContextOptions<RecruiterContext> options)
            : base(options)
        {
        }

        public DbSet<Recruiter> Recruiter { get; set; }
    }
}
