using Microsoft.AspNetCore.Mvc;
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
    }
}
