﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecruiterPathway.Models;
using RecruiterPathway.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RecruiterPathway.Repository;

namespace RecruiterPathway.Controllers
{
    public class RecruitersController : Controller
    {
        private readonly IRecruiterRepository repository;
        private readonly IConfiguration _configuration;

        public RecruitersController(IRecruiterRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var recruiters = await repository.GetAll();

            var recruiterVM = new RecruiterViewModel
            {
                Recruiters = recruiters
            };

            return View(recruiterVM);
        }
        
        // GET: Recruiters/List
        public async Task<IActionResult> List()
        {
            var recruiters = await repository.GetAll();

            var recruiterVM = new RecruiterViewModel
            {
                Recruiters = recruiters
            };

            return View(recruiterVM);
        }

        public async Task<IActionResult> Logout()
        {
            repository.SignOutRecruiter();
            return RedirectToAction(nameof(Index));
        }
        // GET: Recruiters/Login
        public async Task<IActionResult> Login(string returnurl, bool? error)
        {
            ViewData["returnurl"] = returnurl;
            if(error != null)
                ViewData["HasError"] = error;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,Password,RememberMe")] Recruiter model, string returnurl)
        {
            //Find the matching user from the DB
            var result = repository.SignInRecruiter(model);
            //Check if user exists and if password is valid
            if (result.Result)
            {
                //Return that auth was sucessful and assign the token
                if (returnurl != null)
                {
                    //Redirect to the returnurl, from the Login's from paramater, from the Authorize redirect
                    return Redirect(returnurl);
                }
                else 
                {
                    return RedirectToAction(nameof(Profile));
                }
            }
            //AUTH FAIL
            if(returnurl != null) {
                return Redirect("~/Recruiters/Login?error=true&returnurl=" + returnurl);
            }
            return Redirect("~/Recruiters/Login?error=true");

        }
        // GET: Recruiters/Details/5
        [Authorize]
        public async Task<IActionResult> Profile(string id)
        {
            var recruiter = await repository.GetSignedInRecruiter(HttpContext.User);

            if (recruiter == null)
            {
                return NotFound();
            }

            var recruiterVM = new RecruiterViewModel
            {
                Recruiter = recruiter
            };

            return View(recruiterVM);
        }

        // GET: Recruiters/Create
        public IActionResult Create(string errormessage)
        {
            if (errormessage != null)
            {
                //Convert the fake newlines to true newlines
                string error = "";
                foreach (var chr in errormessage)
                {
                    if (chr == '|')
                    {
                        error += '\n';
                    }
                    else
                    {
                        error += chr;
                    }
                }
                ViewData["error-message"] = error;
            }
            return View();
        }

        // POST: Recruiters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CompanyName,UserName,PhoneNumber,PasswordHash")] Recruiter recruiter)
        {
            if (ModelState.IsValid)
            {
                var userExists = await repository.GetRecruiterByName(recruiter.Name);
                if (userExists != null)
                {
                    var recruiterVM = new RecruiterViewModel
                    {
                        Recruiter = recruiter
                    };

                    return View(recruiterVM);
                }

                Recruiter rec = new Recruiter()
                {
                    UserName = recruiter.UserName,
                    Id = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = recruiter.UserName,
                    Name = recruiter.Name,
                    CompanyName = recruiter.CompanyName,
                    PhoneNumber = recruiter.PhoneNumber,
                    Password = recruiter.PasswordHash
                };
                repository.Insert(rec);
                repository.Save();
                return RedirectToAction(nameof(Profile));
            }
            else 
            {
                var errorMessage = "";
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errorMessage += error.ErrorMessage;
                        errorMessage += '|';
                    }
                }
                return Redirect("~/Recruiters/Create/?errormessage="+ errorMessage);
            }
        }

        // GET: Recruiters/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var recruiter = await repository.GetSignedInRecruiter(HttpContext.User);
            if (recruiter == null)
            {
                return NotFound();
            }

            var recruiterVM = new RecruiterViewModel
            {
                Recruiter = recruiter
            };

            return View(recruiterVM);
        }

        // POST: Recruiters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, [Bind("Name,CompanyName,UserName,PhoneNumber")] Recruiter recruiter)
        {
            var userExists = await repository.GetById(id);
            if (userExists == null)
            {
                var recruiterVM = new RecruiterViewModel
                {
                    Recruiter = recruiter
                };

                return View(recruiterVM);
            }

            if (ModelState.IsValid)
            {
                //Sync the complete model with the provided
                userExists.Name = recruiter.Name;
                userExists.CompanyName = recruiter.CompanyName;
                userExists.Email = recruiter.UserName;
                //Sync up username and email
                userExists.UserName = recruiter.UserName;
                //userExists.PasswordHash = model.PasswordHash;
                userExists.PhoneNumber = recruiter.PhoneNumber;
                repository.Update(userExists);
                repository.Save();
                
                return RedirectToAction(nameof(List));
            }
            return RedirectToAction(nameof(List));
        }

        // GET: Recruiters/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruiter = await repository.GetById(id);
            if (recruiter == null)
            {
                return NotFound();
            }

            var recruiterVM = new RecruiterViewModel
            {
                Recruiter = recruiter
            };

            return View(recruiterVM);
        }

        // POST: Recruiters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var self = await repository.GetSignedInRecruiter(HttpContext.User);
            var recruiter = await repository.GetById(id);
            if (self != null && id == self.Id)
            {
                repository.Delete(id);
                repository.SignOutRecruiter();
                repository.Save();
            }
            else {
                return Unauthorized();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
