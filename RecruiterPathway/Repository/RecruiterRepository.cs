﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        override async public ValueTask<Recruiter> GetById(object id)
        {
            return await context.Recruiter.Include(p => p.PipelineStatuses).Include(w => w.WatchList).FirstOrDefaultAsync(r => r.Id == (string)id);
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

        override async public Task<bool> SetPipelineStatus(string recruiterId, string studentId, string status)
        {
            var recruiter = await GetById(recruiterId);
            var pstatus = recruiter.PipelineStatuses.Where(r => r.RecruiterId == recruiterId).FirstOrDefault(r => r.StudentId == studentId);
            if (pstatus == null)
            {
                context.PipelineStatus.Add(new PipelineStatus(studentId, status));
                return true;
            }
            context.PipelineStatus.Remove(pstatus);
            context.PipelineStatus.Add(new PipelineStatus(studentId, status));
            return true;
        }
        override async public Task<bool> SetPipelineStatus(Recruiter recruiter, Student student, string status)
        {
            return await SetPipelineStatus(recruiter.Id, student.Id, status);
        }
        override async public Task AddWatch(Recruiter recruiter, Student student)
        {
            await AddWatch(recruiter.Id, student.Id);
        }
        override async public Task AddWatch(string recruiterId, string studentId)
        {
            var recruiter = await GetById(recruiterId);
            if (!recruiter.WatchList.Contains(studentId))
            {
                recruiter.WatchList.Add(studentId);
            }
        }
        override async public Task RemoveWatch(Recruiter recruiter, Student student) 
        {
            await RemoveWatch(recruiter.Id, student.Id);
        }
        override async public Task RemoveWatch(string recruiterId, string studentId)
        {
            var recruiter = await GetById(recruiterId);
            recruiter.WatchList.Remove(studentId);
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
