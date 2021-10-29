using Microsoft.AspNetCore.Mvc;
using RecruiterPathway.Models;
using System.Threading;
using Xunit;

namespace RecruiterPathway.Tests
{
    public class RecruiterControllerTests
    {
        [Fact]
        public static void GetProfile()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Profile(MockedDatabase.GetRandomGuid());

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetCreate()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Create("");

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
        }
        [Fact]
        public static void GetEdit()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Edit(MockedDatabase.GetRandomGuid());

            Assert.NotNull(result.Result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetDelete()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Delete(MockedDatabase.GetRandomGuid());

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetLogin()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Login("", false);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
        }
        [Fact]
        public static void GetLogout()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Logout();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
        }
        [Fact]
        public static void GetList()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.List();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void PostLogin()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Login(new Recruiter {
                UserName = "leigh.sevitt@brightdog.com",
                Password = "3y8dpcj65!Ar8",
                RememberMe = false
            }, "");

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void PostCreate()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Create(new Recruiter
            {
                UserName = "test@test.com",
                PasswordHash = "Test123!",
                CompanyName = "Test",
                PhoneNumber = "5555555555",
                Name = "Test User"
            });

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void PostEdit()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.Edit(MockedDatabase.GetRandomGuid(), new Recruiter
            {
                UserName = "test@test.com",
                CompanyName = "Test",
                PhoneNumber = "5555555555",
                Name = "Test User"
            });

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void PostDelete()
        {
            var controller = MockedDatabase.GetRecruitersController();

            var result = controller.DeleteConfirmed(MockedDatabase.GetRandomGuid());

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
    }
}
