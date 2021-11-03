using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Models;
using System;

namespace RecruiterPathway.Data
{
    public class DatabaseContext : IdentityDbContext<Recruiter>
    {
        private DbContextOptions<DatabaseContext> options;
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){
            this.options = options;
        }
        public DatabaseContext() { }
        protected DatabaseContext(DatabaseContext context) : base(context.options)
        {
            this.Recruiter = context.Recruiter;
            this.Student = context.Student;
            this.options = context.options;
        }
        public virtual DbSet<Recruiter> Recruiter { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        //Workaround since dependency injection did not like the copy constructor public.
        public DatabaseContext Copy()
        {
            return new DatabaseContext(this);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Yay for stackoverflow https://stackoverflow.com/a/53703521
            builder.Entity<Recruiter>()
                .Property(p => p.WatchList)
                .HasConversion(e => string.Join('|', e), e => e.Split('|', StringSplitOptions.RemoveEmptyEntries));
            base.OnModelCreating(builder);
        }

    }
}
