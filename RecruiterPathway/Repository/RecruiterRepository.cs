﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public class RecruiterRepository : IRecruiterRepository
    {
        private readonly UserManager<Recruiter> _userManager;
        private readonly SignInManager<Recruiter> _authManager;

        public RecruiterRepository(DatabaseContext context, UserManager<Recruiter> userManager, SignInManager<Recruiter> authManager) : base(context)
        {
            this._context = context;
            this._userManager = userManager;
            this._authManager = authManager;
        }

        override async public Task<bool> Insert(Recruiter recruiter) 
        {
            await _userManager.CreateAsync(recruiter, recruiter.PasswordHash);
            return true;
        }

        override public async ValueTask<Recruiter> GetById(object id)
        {
            Recruiter recruiter;
            lock (_context)
            {
                //ThenInclude from https://stackoverflow.com/a/53133582
                recruiter = _context.Recruiter.Include(p => p.PipelineStatuses)
                                        .ThenInclude(s => s.Student)
                                        .Include(w => w.WatchList)
                                        .ThenInclude(s => s.Student)
                                        .SingleOrDefault(r => r.Id == (string)id);
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
            await _authManager.SignOutAsync();
        }
        override async public Task<bool> SignInRecruiter(Recruiter recruiter)
        {
            var result = await _authManager.PasswordSignInAsync(recruiter.UserName, recruiter.Password, recruiter.RememberMe, false);
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
            var recruiter = await _userManager.GetUserAsync(principal);
            //var recruiter = await GetById(recruiterBasedOnSignin.Id);
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

        override async public Task<Recruiter> GetSignedInRecruiter(System.Security.Claims.ClaimsPrincipal principal, bool getExtras)
        {
            var recruiterBasedOnSignin = await _userManager.GetUserAsync(principal);
            var recruiter = await GetById(recruiterBasedOnSignin.Id);
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

        override async public Task<Recruiter> GetRecruiterByName(string name)
        {
            var recruiter =  await _userManager.FindByNameAsync(name);
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

        override async public Task<bool> SetPipelineStatus(string recruiterId, Student student, string status)
        {
            var recruiter = await GetById(recruiterId);
            if (recruiter == null)
            {
                return false;
            }
            var pstatus = recruiter.PipelineStatuses.Where(r => r.Recruiter == recruiter).Where(r => r.Student == student);
            if (pstatus == null || !pstatus.Any())
            {
                _context.PipelineStatus.Add(new PipelineStatus(student, recruiter, status));
                Save();
                return true;
            }
            _context.PipelineStatus.Remove(pstatus.First());
            Save();
            _context.PipelineStatus.Add(new PipelineStatus(student, recruiter, status));
            Save();
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
                Debug.WriteLine("hit null recruiter in AddWatch");
                return;
            }
            var watch = new Watch { Recruiter = recruiter, Id = Guid.NewGuid().ToString(), Student = student };
            if (!_context.WatchList.Where(w => w.Recruiter == recruiter).Where(s => s.Student == student).Any())
            {
                _context.WatchList.Add(watch);
                Save();
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
           _context.WatchList.Remove(recruiter.WatchList.FirstOrDefault(w => w.Student == student));
           Save();
        }   
    }
}
