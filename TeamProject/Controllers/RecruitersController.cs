using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;
using TeamProject.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TeamProject.Controllers
{
    public class RecruitersController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<Recruiter> userManager;
        //private readonly RoleManager<AuthLevels> roleManager;
        private readonly SignInManager<Recruiter> authManager;
        private readonly IConfiguration _configuration;

        public RecruitersController(DatabaseContext context, UserManager<Recruiter> userManager, SignInManager<Recruiter> authManager, IConfiguration configuration)
        {
            _context = context;
            this.userManager = userManager;
            //this.roleManager = roleManager;
            this.authManager = authManager;
            _configuration = configuration;
        }

        // GET: Recruiters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Recruiter.ToListAsync());
        }
        // GET: Recruiters/List
        public async Task<IActionResult> List()
        {
            return View(await _context.Recruiter.ToListAsync());
        }

        public async Task<IActionResult> Logout()
        {
            await authManager.SignOutAsync();
            HttpContext.Session.Remove("Id");
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
            var result = await authManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            //Check if user exists and if password is valid
            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                HttpContext.Session.SetString("Id", user.Id);
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
            var recruiter = await userManager.GetUserAsync(HttpContext.User);

            if (recruiter == null)
            {
                return NotFound();
            }

            return View(recruiter);
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
        public async Task<IActionResult> Create([Bind("Name,CompanyName,UserName,PhoneNumber,PasswordHash")] Recruiter model)
        {
            if (ModelState.IsValid)
            {
                var userExists = await userManager.FindByNameAsync(model.UserName);
                if (userExists != null)
                    return View(model);
                Recruiter recruiter = new Recruiter()
                {
                    UserName = model.UserName,
                    Id = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = model.UserName,
                    Name = model.Name,
                    CompanyName = model.CompanyName,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.PasswordHash
                };
                var result = await userManager.CreateAsync(recruiter, model.PasswordHash);
                if (!result.Succeeded)
                    return View(model);
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
            var recruiter = await userManager.GetUserAsync(HttpContext.User);
            if (recruiter == null)
            {
                return NotFound();
            }
            return View(recruiter);
        }

        // POST: Recruiters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, [Bind("Name,CompanyName,UserName,PhoneNumber")] Recruiter model)
        {
            var userExists = await userManager.FindByIdAsync(id);
            if (userExists == null)
                return View(model);

            Console.WriteLine("Username: " + model.UserName);
            Console.WriteLine("Name: " + model.Name);

            if (ModelState.IsValid)
            {
                //Sync the complete model with the provided
                userExists.Name = model.Name;
                userExists.CompanyName = model.CompanyName;
                userExists.Email = model.UserName;
                //Sync up username and email
                userExists.UserName = model.UserName;
                //userExists.PasswordHash = model.PasswordHash;
                userExists.PhoneNumber = model.PhoneNumber;
                await userManager.UpdateAsync(userExists);
                
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

            var recruiter = await _context.Recruiter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recruiter == null)
            {
                return NotFound();
            }

            return View(recruiter);
        }

        // POST: Recruiters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var self = await userManager.GetUserAsync(HttpContext.User);
            var recruiter = await _context.Recruiter.FindAsync(id);
            if (self != null && id == self.Id)
            {
                _context.Recruiter.Remove(recruiter);
                await authManager.SignOutAsync();
                await _context.SaveChangesAsync();
            }
            else {
                return Unauthorized();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
