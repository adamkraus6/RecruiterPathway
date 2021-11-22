using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecruiterPathway.Controllers;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using RecruiterPathway.Repository;
using RecruiterPathway.Tests.Fixture;

namespace RecruiterPathway.Tests
{
    class MockedDatabase
    {
        private static List<string> guids = null;
        private static DatabaseContext dbContext = null;
        private static bool seeded = false;
        public static DatabaseContext GetDatabaseContext()
        {
            var fixture = new SharedDatabaseFixture();
            return fixture.CreateContext();
        }

        public static Mock<UserManager<Recruiter>> GetRecruiterUserManager(DatabaseContext context)
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
                var db = context;
                guids = new List<string>();
                //A. Helps with more async potential fixes. B. Makes me happier
                lock (db) lock(guids)
                    {
                        SeedDatabase.SeedRecruiters(db, userManagerMock.Object, ref guids);
                        dbContext = db;
                    }
            }
            return userManagerMock;
        }
        //Needed https://stackoverflow.com/a/64832602 to help fix this
        public static Mock<SignInManager<Recruiter>> GetRecruiterSignInManager()
        {
            return new Mock<SignInManager<Recruiter>>(
            GetRecruiterUserManager(GetDatabaseContext()).Object, 
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
            return new RecruiterRepository(GetDatabaseContext(), GetRecruiterUserManager(GetDatabaseContext()).Object, GetRecruiterSignInManager().Object);
        }
        public static StudentRepository GetStudentRepository()
        {
            //Test to see if we've seeded the recruiter repo, resolves some linking issues
            if (guids == null || guids.Count == 0)
            {
                GetRecruiterRepository();
            }
            return new StudentRepository(GetDatabaseContext());
        }
        public static StudentsController GetStudentsController()
        {
            return new StudentsController(GetStudentRepository(), GetRecruiterRepository());
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
