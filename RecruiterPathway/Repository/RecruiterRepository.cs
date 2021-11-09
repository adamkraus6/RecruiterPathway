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
            await userManager.CreateAsync(recruiter, recruiter.PasswordHash);
            return true;
        }

        override public async ValueTask<Recruiter> GetById(object id)
        {
            Recruiter recruiter;
            lock (context)
            {
                //ThenInclude from https://stackoverflow.com/a/53133582
                recruiter = context.Recruiter.Include(p => p.PipelineStatuses)
                                        .ThenInclude(s => s.Student)
                                        .Include(w => w.WatchList)
                                        .ThenInclude(s => s.Student)
                                        .FirstOrDefault(r => r.Id == (string)id);
            }
            if (recruiter.WatchList == null)
            {
                recruiter.WatchList = new List<Watch>();
            }
            if (recruiter.PipelineStatuses == null)
            {
                recruiter.PipelineStatuses = new List<PipelineStatus>();
            }
            return recruiter;
        }
        override async public void SignOutRecruiter()
        {
            await authManager.SignOutAsync();
        }
        override async public Task<bool> SignInRecruiter(Recruiter recruiter)
        {
            var result = await authManager.PasswordSignInAsync(recruiter.UserName, recruiter.Password, recruiter.RememberMe, false);
            if (result != null && result.Succeeded)
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

        override async public Task<bool> SetPipelineStatus(string recruiterId, Student student, string status)
        {
            var recruiter = await GetById(recruiterId);
            if (recruiter == null)
            {
                return false;
            }
            var pstatus = recruiter.PipelineStatuses.Where(r => r.Recruiter == recruiter).FirstOrDefault(r => r.Student == student);
            if (pstatus == null)
            {
                context.PipelineStatus.Add(new PipelineStatus(student, recruiter, status));
                return true;
            }
            context.PipelineStatus.Remove(pstatus);
            context.PipelineStatus.Add(new PipelineStatus(student, recruiter, status));
            return true;
        }
        override async public Task<bool> SetPipelineStatus(Recruiter recruiter, Student student, string status)
        {
            return await SetPipelineStatus(recruiter.Id, student, status);
        }
        override async public Task AddWatch(Recruiter recruiter, Student student)
        {
            await AddWatch(recruiter.Id, student);
        }
        override async public Task AddWatch(string recruiterId, Student student)
        {
            var recruiter = await GetById(recruiterId);
            if (recruiter == null)
            {
                return;
            }
            if (recruiter.WatchList == null)
            {
                recruiter.WatchList = new List<Watch>();
            }
            var watch = new Watch { Recruiter = recruiter, Id = Guid.NewGuid().ToString(), Student = student };
            if (!context.WatchList.Contains(watch))
            {
                context.WatchList.Add(watch);
            }
        }
        override async public Task RemoveWatch(Recruiter recruiter, Student student) 
        {
            await RemoveWatch(recruiter.Id, student);
        }
        override async public Task RemoveWatch(string recruiterId, Student student)
        {
            var recruiter = await GetById (recruiterId);
            if (recruiter == null)
            {
                return;
            }
           context.WatchList.Remove(recruiter.WatchList.FirstOrDefault(w => w.Student == student));
        }   
    }
}
