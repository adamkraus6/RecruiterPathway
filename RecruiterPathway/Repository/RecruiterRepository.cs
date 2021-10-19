using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public class RecruiterRepository : IRecruiterRepository, IDisposable
    {
        private DatabaseContext context;
        private readonly UserManager<Recruiter> userManager;
        private readonly SignInManager<Recruiter> authManager;

        public RecruiterRepository(DatabaseContext context, UserManager<Recruiter> userManager, SignInManager<Recruiter> authManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.authManager = authManager;
        }

        async Task<IEnumerable<Recruiter>> IRecruiterRepository.GetRecruiters() 
        {
            return await context.Recruiter.ToListAsync();
        }
        async Task<Recruiter> IRecruiterRepository.GetRecruiterById(string id) 
        {
            return await context.Recruiter.FindAsync(id).AsTask();
        }
        async void IRecruiterRepository.InsertRecruiter(Recruiter recruiter) 
        {
            if (IsValid(recruiter))
                await userManager.CreateAsync(recruiter, recruiter.PasswordHash);
        }
        async void IRecruiterRepository.DeleteRecruiter(string id) 
        {
            if (exists(id))
            {
                Recruiter student = (await context.Recruiter.FindAsync(id).AsTask());
                context.Recruiter.Remove(student);
            }
        }
        void IRecruiterRepository.UpdateRecruiter(Recruiter recruiter) 
        {
            context.Entry(recruiter).State = EntityState.Modified;
        }
        void IRecruiterRepository.Save()
        {
            context.SaveChanges();
        }
        async void IRecruiterRepository.SignOutRecruiter()
        {
            await authManager.SignOutAsync();
        }
        async Task<bool> IRecruiterRepository.SignInRecruiter(Recruiter recruiter)
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

        async Task<Recruiter> IRecruiterRepository.GetSignedInRecruiter(System.Security.Claims.ClaimsPrincipal principal)
        {
            return await userManager.GetUserAsync(principal);
        }

        async Task<Recruiter> IRecruiterRepository.GetRecruiterByName(string name)
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

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
    }
}
