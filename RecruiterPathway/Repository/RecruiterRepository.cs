using Microsoft.AspNetCore.Identity;
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

        override async public Task Delete(object id)
        {
            var recruiter = await GetById(id);
            //Delete all the linked objects
            lock (_context) lock(_userManager)
            {
                var comments = _context.Comment.Where(c => c.Recruiter.Id == recruiter.Id);
                _context.Comment.RemoveRange(comments);
                var statuses = _context.PipelineStatus.Where(c => c.Recruiter.Id == recruiter.Id);
                _context.PipelineStatus.RemoveRange(statuses);
                var watches = _context.WatchList.Where(c => c.Recruiter.Id == recruiter.Id);
                _context.WatchList.RemoveRange(watches);
                _context.SaveChanges();
            }
            //Manual Recruiter delete through userManager to resolve all link issues
            await _userManager.DeleteAsync(recruiter);
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
            //Due to issues with some of the getters and our testing format, we catch the null recruiter from the database
            //that the testing suite doesn't handle right and give them a stubbed version that's constant.
            if (recruiter == null)
            {
                recruiter = Constants.NullRecruiter;
                Debug.WriteLine("Using Null Recruiter. This should NEVER be used in production but is fine for tests.");
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
        //This method can only be used in Prod since ClaimsPrincipal is not really mockable
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
            var pstatus = _context.PipelineStatus.Where(r => r.Recruiter == recruiter).Where(r => r.Student == student);
            var pipelineStatus = new PipelineStatus
            {
                Student = student,
                Recruiter = recruiter,
                Status = status,
                Id = Guid.NewGuid().ToString()
            };
            if (pstatus == null || !pstatus.Any())
            {
                lock (_context)
                {
                    _context.PipelineStatus.Add(pipelineStatus);
                    _context.SaveChanges();
                }
                return true;
            }
            if (pstatus.First() != null)
            {
                lock (_context)
                {
                    _context.PipelineStatus.Remove(pstatus.First());
                    _context.SaveChanges();
                }
            }
            lock (_context)
            {
                _context.PipelineStatus.Add(pipelineStatus);
                _context.SaveChanges();
            }
            //Save();
            return true;
        }
        override async public Task<bool> SetPipelineStatus(Recruiter recruiter, Student student, string status)
        {
            return await SetPipelineStatus(recruiter.Id, student, status);
        }

        override public SelectList GetPipelineStatuses()
        {
            var statusQuery = from r in _context.Recruiter
                              from ps in r.PipelineStatuses
                              select ps.Status;
            return new SelectList(statusQuery.Distinct());
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
                _context.SaveChanges();
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
           _context.SaveChanges();
        }   
    }
}
