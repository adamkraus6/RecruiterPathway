using Microsoft.AspNetCore.Identity;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public abstract class IRecruiterRepository : GenericRepository<Recruiter>
    {
        protected IRecruiterRepository(DatabaseContext context) : base(context, context.Recruiter) { }
        public abstract void SignOutRecruiter();
        public abstract Task<bool> SignInRecruiter(Recruiter recruiter);
        public abstract Task<Recruiter> GetSignedInRecruiter(ClaimsPrincipal principal);
        public abstract Task<Recruiter> GetRecruiterByName(string name);

        //Pipeline Status methods, setup this way since IDK how the actual final setup will work
        public abstract Task<PipelineStatus> GetPipelineStatus(Recruiter recruiter, Student student);
        public abstract Task<PipelineStatus> GetPipelineStatus(string recruiterId, Student student);
        public abstract Task<PipelineStatus> GetPipelineStatus(Recruiter recruiter, string studentId);
        public abstract Task<PipelineStatus> GetPipelineStatus(string recruiterId, string studentId);
        public abstract Task<bool> SetPipelineStatus(Recruiter recruiter, Student student, string sstatus);
        public abstract Task<bool> SetPipelineStatus(string recruiterId, string studentId, string status);

        //Watchlist Functions - Enforces Only 1 in the list
        public abstract Task AddWatch(Recruiter recruiter, Student student);
        public abstract Task AddWatch(string recruiterId, string studentId);
        public abstract Task RemoveWatch(Recruiter recruiter, Student student);
        public abstract Task RemoveWatch(string recruiterId, string studentId);

    }
}
