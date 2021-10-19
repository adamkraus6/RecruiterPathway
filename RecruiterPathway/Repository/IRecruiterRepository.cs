using Microsoft.AspNetCore.Identity;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public abstract class IRecruiterRepository : GenericRepository<Recruiter>, IDisposable
    {
        public IRecruiterRepository(DatabaseContext context) : base(context, context.Recruiter) { }
        public abstract void SignOutRecruiter();
        public abstract Task<bool> SignInRecruiter(Recruiter recruiter);
        public abstract Task<Recruiter> GetSignedInRecruiter(ClaimsPrincipal principal);
        public abstract Task<Recruiter> GetRecruiterByName(string name);
    }
}
