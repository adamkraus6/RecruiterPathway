using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TeamProject.Data;

namespace TeamProject.Models
{
    public class SeedDatabase
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new DatabaseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<DatabaseContext>>()))
            {
                // Look for any movies.
                if (context.Student.Any())
                {
                    return;   // DB has been seeded
                }

                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "TeamProject.Models.SeedData.Students.csv";
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
        }
    }
}
