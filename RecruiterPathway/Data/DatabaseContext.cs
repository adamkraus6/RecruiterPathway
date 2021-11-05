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
            this.PipelineStatus = context.PipelineStatus;
            this.options = context.options;
        }
        public virtual DbSet<Recruiter> Recruiter { get; set; }
        public virtual DbSet<Student> Student { get; set; }

        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<PipelineStatus> PipelineStatus { get; set; }
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
            builder.Entity<PipelineStatus>()
                .HasOne(r => r.Recruiter)
                .WithMany(p => p.PipelineStatuses)
                .HasForeignKey(r => r.RecruiterId)
                .OnDelete(DeleteBehavior.Cascade);
            //ToTable from https://stackoverflow.com/a/60958165 in relation to crashing on adding a comment
            builder.Entity<Comment>()
                .ToTable("Comment")
                .HasOne(s => s.Student)
                .WithMany(b => b.Comments)
                .HasForeignKey(i => i.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Student>()
                .HasMany(c => c.Comments)
                .WithOne(s => s.Student)
                .HasForeignKey(i => i.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
        }

    }
}
