using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;
using TeamProject.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TeamProject.Controllers
{
    public class RecruitersController : Controller
    {
        private readonly AuthenticationDbContext _context;
        private readonly UserManager<Recruiter> userManager;
        private readonly RoleManager<AuthLevels> roleManager;
        private readonly IConfiguration _configuration;

        public RecruitersController(AuthenticationDbContext context, UserManager<Recruiter> userManager, RoleManager<AuthLevels> roleManager, IConfiguration configuration)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
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
            var user = await userManager.FindByNameAsync(model.UserName);

            //Check if user exists and if password is valid
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                //Not much clue what this does, setup authorization claims
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                //Get our private key from the config
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                //Create the actual authentication token
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                //Return that auth was sucessful and assign the token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            //AUTH FAIL
            return Unauthorized();
        }
        // GET: Recruiters/Details/5
        public async Task<IActionResult> Profile(string id)
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
                Password = model.Password
            };
            //Console.WriteLine(recruiter.ToString());
            //Hash is NOT hash, it is plan text.
            //TODO: Implement Hashing
            var result = await userManager.CreateAsync(recruiter, model.Password);
            if (!result.Succeeded)
                return View(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: Recruiters/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruiter = await _context.Recruiter.FindAsync(id);
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
            var recruiter = await _context.Recruiter.FindAsync(id);
            _context.Recruiter.Remove(recruiter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecruiterExists(string id)
        {
            return _context.Recruiter.Any(e => e.Id == id);
        }
    }
}
