using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using RecruiterPathway.Data;
using Microsoft.AspNetCore.Identity;

namespace RecruiterPathway.Models
{
    public class SeedDatabase
    {
        public static void Initialize(IServiceProvider serviceProvider, UserManager<Recruiter> userManager)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "";
;            using (var context = new DatabaseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<DatabaseContext>>()))
            {
                // Look for any movies.
                if (!context.Student.Any())
                {
                    // DB has been seeded


                    resourceName = "RecruiterPathway.Models.SeedData.Students.csv";
                    string line;
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            // Writes to the Output Window.
                            Debug.WriteLine(line);

                            // Logic to parse the line, separate by comma(s), and assign fields
                            // to the Student model.
                            string[] values = line.Split(',');

                            context.Student.AddRange(
                                new Student
                                {
                                    firstName = values[0],
                                    lastName = values[1],
                                    degree = values[2],
                                    gradDate = DateTime.Parse(values[3])
                                }
                            );

                            context.SaveChanges();
                        }
                    }
                }
                //Seed the administrative user into the database
                var adminuser = new Recruiter
                {
                    UserName = "administrator@recruiterpathway.com",
                    Email = "administrator@recruiterpathway.com",
                    Name = "Administrator",
                    CompanyName = "Recruiter Pathway",
                    PhoneNumber = "6055555555",
                    Password = "P@$$w0rd",
                    Id = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                userManager.DeleteAsync(adminuser);
                try
                {
                    userManager.CreateAsync(adminuser, "P@$$w0rd");
                }
                catch (DbUpdateException ex)
                {
                    //We expect this to fail, try to add the user anyways.
                }
                resourceName = "RecruiterPathway.Models.SeedData.Recruiters.csv";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    reader.ReadLine();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Debug.WriteLine(line);
                        string[] values = line.Split(',');

                        var data_recruiter =
                            new Recruiter
                            {
                                Id = values[0],
                                Name = values[1],
                                CompanyName = values[2],
                                PhoneNumber = values[3],
                                Email = values[1].Replace(' ','.') + "@" + values[2].ToLower() + ".com",
                                UserName = values[1].Replace(' ', '.') + "@" + values[2].ToLower() + ".com",
                                Password = values[4] + "!8"
                            };
                        var test = context.Recruiter.Where(r => r.Email == values[1] + "@" + values[2].ToLower() + ".com");
                        if (test != null) {
                            try
                            {
                                userManager.CreateAsync(data_recruiter, values[4] + "!8");
                            }
                            catch (DbUpdateException ex)
                            {
                                //Tell it we expect this to come up every so often, likely due to processing speed and to ignore it
                            }
                        }
                    }
                }

            }




        }
    }
}
