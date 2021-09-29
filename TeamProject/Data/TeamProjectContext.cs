using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamProject.Models;

namespace TeamProject.Data
{
    public class TeamProjectContext : DbContext
    {
        public TeamProjectContext (DbContextOptions<TeamProjectContext> options)
            : base(options)
        {
        }

        public DbSet<TeamProject.Models.Student> Student { get; set; }
    }
}
