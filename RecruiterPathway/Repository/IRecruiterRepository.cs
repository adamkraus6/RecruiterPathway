using Microsoft.AspNetCore.Identity;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public interface IRecruiterRepository : IDisposable
    {
        Task<IEnumerable<Recruiter>> GetRecruiters();
        Task<Recruiter> GetRecruiterById(string id);
        void InsertRecruiter(Recruiter recruiter);
        void DeleteRecruiter(string id);
        void UpdateRecruiter(Recruiter recruiter);
        void SignOutRecruiter();
        Task<bool> SignInRecruiter(Recruiter recruiter);
        Task<Recruiter> GetSignedInRecruiter(ClaimsPrincipal principal);
        Task<Recruiter> GetRecruiterByName(string name);
        void Save();

    }
}
