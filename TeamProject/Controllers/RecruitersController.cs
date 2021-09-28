using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;
using TeamProject.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace TeamProject.Controllers
{
    public class RecruitersController : Controller
    {
        private readonly AuthenticationDbContext _context;
        private readonly UserManager<Recruiter> userManager;
        //private readonly RoleManager<AuthLevels> roleManager;
        private readonly SignInManager<Recruiter> authManager;
        private readonly IConfiguration _configuration;

        public RecruitersController(AuthenticationDbContext context, UserManager<Recruiter> userManager, SignInManager<Recruiter> authManager, IConfiguration configuration)
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
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,Password")] Recruiter model)
        {
            //Find the matching user from the DB
            var result = await authManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            //Check if user exists and if password is valid
            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                HttpContext.Session.SetString("Id", user.Id);
                Console.WriteLine(HttpContext.User.GetType().ToString());
                //Return that auth was sucessful and assign the token
                return RedirectToAction(nameof(Profile));
            }
            //AUTH FAIL
            return RedirectToAction(nameof(Login));
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recruiters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CompanyName,UserName,PasswordHash")] Recruiter model)
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
                Password = model.PasswordHash
            };
            var result = await userManager.CreateAsync(recruiter, model.PasswordHash);
            if (!result.Succeeded)
                return View(model);
            return RedirectToAction(nameof(Profile));
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
        public async Task<IActionResult> Edit(string id, [Bind("Name,CompanyName,UserName,PasswordHash")] Recruiter model)
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
                userExists.PasswordHash = model.PasswordHash;
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
