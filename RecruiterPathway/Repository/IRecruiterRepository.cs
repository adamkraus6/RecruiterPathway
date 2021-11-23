using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public abstract Task<Recruiter> GetSignedInRecruiter(ClaimsPrincipal principal, bool withExtras);
        public abstract Task<Recruiter> GetRecruiterByName(string name);

        //Pipeline Status methods, setup this way since IDK how the actual final setup will work
        public abstract Task<bool> SetPipelineStatus(Recruiter recruiter, Student student, string status);
        public abstract Task<bool> SetPipelineStatus(string recruiterId, Student student, string status);

        //Watchlist Functions - Enforces Only 1 in the list
        public abstract Task AddWatch(Recruiter recruiter, Student student);
        public abstract Task AddWatch(string recruiterId, Student student);
        public abstract Task RemoveWatch(Recruiter recruiter, Student student);
        public abstract Task RemoveWatch(string recruiterId, Student student);

    }
}
