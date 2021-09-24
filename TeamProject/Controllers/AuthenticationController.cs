using TeamProject.Authentication;
using TeamProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;


//TODO: Delete Me once all functions have been fully moved to the Recruiter Controller
namespace TeamProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<Recruiter> userManager;
        private readonly RoleManager<AuthLevels> roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<Recruiter> userManager, RoleManager<AuthLevels> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
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
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Recruiter model)
        {
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest);
            Recruiter recruiter = new Recruiter()
            {
                UserName = model.UserName,
                Id = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.UserName,
                Name = model.Name,
                CompanyName = model.CompanyName
            };
            //Console.WriteLine(recruiter.ToString());
            //Hash is NOT hash, it is plan text.
            //TODO: Implement Hashing
            var result = await userManager.CreateAsync(recruiter, model.PasswordHash);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok();
        }
    }
}
