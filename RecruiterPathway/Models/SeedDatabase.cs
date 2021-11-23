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
                                    Id = values[4],
                                    FirstName = values[0],
                                    LastName = values[1],
                                    Degree = values[2],
                                    GradDate = DateTime.Parse(values[3])
                                }
                            );

                            context.SaveChanges();
                        }
                    }
                }
                //Seed the administrative user into the database
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
                                Id = values[4],
                                FirstName = values[0],
                                LastName = values[1],
                                Degree = values[2],
                                GradDate = DateTime.Parse(values[3]),
                                Comments = new List<Comment>()
                            }
                        );

                        context.SaveChanges();

                    }
                }
            }
        }
        public static void SeedLinkedFields(DatabaseContext context)
        {
            var recruiter = context.Recruiter.Where(r => r.Id == Constants.AdminRecruiter.Id).FirstOrDefault();
            foreach (var student in context.Student.ToList())
            {
                context.Comment.Add(
                    new Comment
                    {
                        Student = student,
                        ActualComment = "Test Comment",
                        Id = Guid.NewGuid().ToString(),
                        Recruiter = recruiter,
                        Time = DateTime.Now
                    }
                );
                context.PipelineStatus.Add(
                    new PipelineStatus
                    {
                        Student = student,
                        Id = Guid.NewGuid().ToString(),
                        Recruiter = recruiter,
                        Status = "Phone Interview"
                    }
                    );
                context.WatchList.Add(
                    new Watch
                    {
                        Student = student,
                        Id = Guid.NewGuid().ToString(),
                        Recruiter = recruiter
                    }
                    );
            }
            context.SaveChanges();
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
            userManager.CreateAsync(Constants.NullRecruiter, "Test123!");
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
