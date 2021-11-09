using Microsoft.AspNetCore.Mvc;
using RecruiterPathway.Models;
using RecruiterPathway.ViewModels;
using System;
using Xunit;

namespace RecruiterPathway.Tests
{
    public class StudentControllerTests
    {
        [Fact]
        public static void GetIndex()
        {
            var controller = MockedDatabase.GetStudentsController();

            var viewModel = new StudentViewModel
            {
                StudentDegree = "",
                SearchFirstName = "",
                SearchLastName = "",
                GradDateStart = new DateTime(),
                GradDateEnd = new DateTime(),
                ListView = false
            };

            var result = controller.Index(viewModel);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetIndexWithFilter()
        {
            var controller = MockedDatabase.GetStudentsController();

            var viewModel = new StudentViewModel
            {
                StudentDegree = "",
                SearchFirstName = "Ira",
                SearchLastName = "Matzen",
                GradDateStart = new DateTime(),
                GradDateEnd = new DateTime(),
                ListView = false
            };

            var result = controller.Index(viewModel);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetDetails()
        {
            var controller = MockedDatabase.GetStudentsController();

            var result = controller.Details("1");

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetCreate()
        {
            var controller = MockedDatabase.GetStudentsController();

            var result = controller.Create("");

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
        }
        [Fact]
        public static void GetEdit()
        {
            var controller = MockedDatabase.GetStudentsController();

            var result = controller.Edit("1");

            Assert.NotNull(result.Result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetDelete()
        {
            var controller = MockedDatabase.GetStudentsController();

            var result = controller.Delete("1");

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void PostCreate()
        {
            var controller = MockedDatabase.GetStudentsController();

            var viewModel = new StudentViewModel
            {
                Student = new Student
                {
                    Id = "9002",
                    firstName = "Test",
                    lastName = "Test"
                }
            };

            var result = controller.Create(viewModel);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void PostEdit()
        {
            var controller = MockedDatabase.GetStudentsController();

            var viewModel = new StudentViewModel
            {
                Student = new Student
                {
                    Id = "1",
                    firstName = "Test",
                    lastName = "Test",
                    degree = "Test",
                    gradDate = new DateTime()
                }
            };

            var result = controller.Edit("1", viewModel);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public async static void PostDelete()
        {
            var controller = MockedDatabase.GetStudentsController();

            var result = await controller.DeleteConfirmed("1");

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
