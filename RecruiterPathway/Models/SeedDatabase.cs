using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using RecruiterPathway.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Collections.Generic;

namespace RecruiterPathway.Models
{
    public class SeedDatabase
    {
        public static void Initialize(IServiceProvider serviceProvider, UserManager<Recruiter> userManager)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "";
            using (var context = new DatabaseContext(
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
                        var i = 1;
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
                                    Id = i.ToString(),
                                    firstName = values[0],
                                    lastName = values[1],
                                    degree = values[2],
                                    gradDate = DateTime.Parse(values[3])
                                }
                            );

                            context.SaveChanges();
                            i++;
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
                    Id = "d2166836-4ce9-4b8e-98dd-b102c00f06f8",
                    SecurityStamp = "32179209-a914-40ad-b052-bff23fe90fc4"
                };
                userManager.DeleteAsync(adminuser);
                try
                {
                    userManager.CreateAsync(adminuser, "P@$$w0rd");
                }
                catch (DbUpdateException)
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
                                Password = values[4] + "!Ar8",
                                WatchList = new List<Watch>(),
                                PipelineStatuses = new List<PipelineStatus>()
                            };
                        var test = context.Recruiter.Where(r => r.Id == values[0]);
                        if (test != null)
                        {
                            try
                            {
                                userManager.CreateAsync(data_recruiter, values[4] + "!Ar8");
                                Thread.Sleep(100);
                            }
                            catch (DbUpdateException)
                            {
                                //Tell it we expect this to come up every so often, likely due to processing speed and to ignore it
                            }
                        }
                        else 
                        {
                            return;
                        }
                    }
                }

            }
        }
        public static void SeedStudents(DatabaseContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "";
            if (!context.Student.Any())
            {
                // DB has been seeded
                resourceName = "RecruiterPathway.Models.SeedData.Students.csv";
                string line;
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    reader.ReadLine();
                    var i = 1;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Writes to the Output Window.
                        Debug.WriteLine(line);

                        // Logic to parse the line, separate by comma(s), and assign fields
                        // to the Student model.
                        string[] values = line.Split(',');

                        IEnumerable<Student> all = context.Student.Where(s => s.Id == i.ToString());
                        if (all.Any())
                        {
                            return;
                        }

                        context.Student.AddRange(
                            new Student
                            {
                                Id = i.ToString(),
                                firstName = values[0],
                                lastName = values[1],
                                degree = values[2],
                                gradDate = DateTime.Parse(values[3]),
                                Comments = new List<Comment>()
                            }
                        );

                        context.SaveChanges();
                        i++;
                    }
                }
            }
        }
        public static void SeedRecruiters(DatabaseContext context, UserManager<Recruiter> userManager, ref List<string> guids) {
            var assembly = Assembly.GetExecutingAssembly();
            var adminuser = Constants.AdminRecruiter;
            userManager.DeleteAsync(adminuser);
            try
            {
                userManager.CreateAsync(adminuser, "P@$$w0rd");
            }
            catch (DbUpdateException)
            {
                //We expect this to fail, try to add the user anyways.
            }
            var resourceName = "RecruiterPathway.Models.SeedData.Recruiters.csv";
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
                            Email = values[1].Replace(' ', '.') + "@" + values[2].ToLower() + ".com",
                            UserName = values[1].Replace(' ', '.') + "@" + values[2].ToLower() + ".com",
                            Password = values[4] + "!Ar8",
                            WatchList = new List<Watch>(),
                            PipelineStatuses = new List<PipelineStatus>()
                        };
                    var test = context.Recruiter.Where(r => r.Id == values[0]);
                    if (test != null)
                    {
                        try
                        {
                            userManager.CreateAsync(data_recruiter, values[4] + "!Ar8");
                            if (guids != null)
                            {
                                guids.Add(values[0]);
                            }
                            Thread.Sleep(100);
                        }
                        catch (DbUpdateException)
                        {
                            //Tell it we expect this to come up every so often, likely due to processing speed and to ignore it
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}
