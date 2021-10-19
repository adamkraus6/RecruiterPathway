using Microsoft.AspNetCore.Identity;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public class RecruiterRepository : IRecruiterRepository, IDisposable
    {
        private readonly UserManager<Recruiter> userManager;
        private readonly SignInManager<Recruiter> authManager;

        public RecruiterRepository(DatabaseContext context, UserManager<Recruiter> userManager, SignInManager<Recruiter> authManager) : base(context)
        {
            this.context = context;
            this.userManager = userManager;
            this.authManager = authManager;
        }

        override async public Task<bool> Insert(Recruiter recruiter) 
        {
            if (IsValid(recruiter))
            {
                await userManager.CreateAsync(recruiter, recruiter.PasswordHash);
                return true;
            }
            return false;
        }
        override async public void Delete(object id) 
        {
            if (exists((string)id))
            {
                Recruiter student = (await context.Recruiter.FindAsync(id).AsTask());
                context.Recruiter.Remove(student);
            }
        }
        override async public void SignOutRecruiter()
        {
            await authManager.SignOutAsync();
        }
        override async public Task<bool> SignInRecruiter(Recruiter recruiter)
        {
            var result = await authManager.PasswordSignInAsync(recruiter.UserName, recruiter.Password, recruiter.RememberMe, false);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        override async public Task<Recruiter> GetSignedInRecruiter(System.Security.Claims.ClaimsPrincipal principal)
        {
            return await userManager.GetUserAsync(principal);
        }

        override async public Task<Recruiter> GetRecruiterByName(string name)
        {
            return await userManager.FindByNameAsync(name);
        }

        private bool IsValid(Recruiter student)
        {
            return true;
        }

        private bool exists(string id)
        {
            return context.Recruiter.Any(e => e.Id == id);
        }       
    }
}
