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

    }
}
