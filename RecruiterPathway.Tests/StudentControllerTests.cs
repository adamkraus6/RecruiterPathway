using Microsoft.AspNetCore.Mvc;
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

            var result = controller.Index("", "", "", new DateTime(), new DateTime(), false);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result.Result);
        }
        [Fact]
        public static void GetIndexWithFilter()
        {
            var controller = MockedDatabase.GetStudentsController();

            var result = controller.Index("", "Ira", "Matzen", new DateTime(), new DateTime(), false);

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
    }
}
