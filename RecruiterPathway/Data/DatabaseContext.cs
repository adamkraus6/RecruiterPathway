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
            this.Comment = context.Comment;
            this.WatchList = context.WatchList;
            this.PipelineStatus = context.PipelineStatus;
            this.options = context.options;
        }
        public virtual DbSet<Recruiter> Recruiter { get; set; }
        public virtual DbSet<Student> Student { get; set; }

        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<PipelineStatus> PipelineStatus { get; set; }
        public virtual DbSet<Watch> WatchList { get; set; }
        //Workaround since dependency injection did not like the copy constructor public.
        public DatabaseContext Copy()
        {
            return new DatabaseContext(this);
        }

    }
}
