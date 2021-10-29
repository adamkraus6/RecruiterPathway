using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecruiterPathway.Controllers;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using RecruiterPathway.Repository;

namespace RecruiterPathway.Tests
{
    class MockedDatabase
    {
        private static List<string> guids = null;
        public static DatabaseContext GetDatabaseContext()
        {
            var db = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "AuthenticationDbContext").Options);
            SeedDatabase.SeedStudents(db);
            return db;
        }
        public static Mock<UserManager<Recruiter>> GetRecruiterUserManager()
        {
            var userManagerMock = new Mock<UserManager<Recruiter>>(
                Mock.Of<IUserStore<Recruiter>>(),
                null,
                null,
                new IUserValidator<Recruiter>[0],
                new IPasswordValidator<Recruiter>[0],
                null,
                null,
                null,
                null
                );
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Recruiter()));
            if (guids == null)
            {
                guids = new List<string>();
                SeedDatabase.SeedRecruiters(GetDatabaseContext(), userManagerMock.Object, ref guids);
            }
            else 
            {
                //Discard the list we generate here since it's the same each time
                var _ = new List<string>();
                SeedDatabase.SeedRecruiters(GetDatabaseContext(), userManagerMock.Object, ref _);
            }
            return userManagerMock;
        }
        //Needed https://stackoverflow.com/a/64832602 to help fix this
        public static Mock<SignInManager<Recruiter>> GetRecruiterSignInManager()
        {
            return new Mock<SignInManager<Recruiter>>(
            GetRecruiterUserManager().Object, 
            Mock.Of<IHttpContextAccessor>(), 
            Mock.Of<IUserClaimsPrincipalFactory<Recruiter>>(),  
            null,  
            null,  
            null,
            null
                );
        }
        public static RecruiterRepository GetRecruiterRepository()
        {
            return new RecruiterRepository(GetDatabaseContext(), GetRecruiterUserManager().Object, GetRecruiterSignInManager().Object);
        }
        public static StudentRepository GetStudentRepository()
        {
            return new StudentRepository(GetDatabaseContext());
        }
        public static StudentsController GetStudentsController()
        {
            return new StudentsController(GetStudentRepository());
        }
        public static RecruitersController GetRecruitersController()
        {
            return new RecruitersController(GetRecruiterRepository());
        }
        public static List<string> GetRecruiterGuids()
        {
            return guids;
        }

        //Quickly get a valid guid that's in the database
        public static string GetRandomGuid()
        {
            var guids = MockedDatabase.GetRecruiterGuids();
            var rand = new Random();
            return guids[rand.Next(0, guids.Count)];
        }
    }
}
