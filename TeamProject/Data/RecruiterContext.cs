using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamProject.Models;

namespace TeamProject.Data
{
    public class RecruiterContext : DbContext
    {
        public RecruiterContext (DbContextOptions<RecruiterContext> options)
            : base(options)
        {
        }

        public DbSet<TeamProject.Models.Recruiter> Recruiter { get; set; }
    }
}
