using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamProject.Data;

namespace TeamProject.Models
{
    public class RecruiterSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AuthenticationDbContext(serviceProvider.GetRequiredService<DbContextOptions<AuthenticationDbContext>>()))
            {
                //Check for Admin Recruiter data. If it doesn't exist, create it.
                if (context.Recruiter.Contains(
                    new Recruiter {}))
                {
                    return;
                }

            }
        }
    }
}
