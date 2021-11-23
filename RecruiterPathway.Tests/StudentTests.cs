using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RecruiterPathway.Tests
{
    public class StudentTests
    {
        [Fact]
        public async Task GetById_Ret_Product()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var idTest = await repository.GetById("7eb89242-712a-444b-a363-61b50477c8a1");
            Assert.NotNull(idTest);
            Assert.IsAssignableFrom<Student>(idTest);
        }
        [Fact]
        public void GetAll_Ret_Product()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var product = repository.GetAll().Result;
            Assert.NotNull(product);
            Assert.IsAssignableFrom<List<Student>>(product);
        }
        [Fact]
        public void Insert_Ret_Product()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var product = repository.Insert(new Student
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "Test",
                Degree = "Test Test",
                GradDate = new DateTime()
            }).Result;
            Assert.IsAssignableFrom<bool>(product);
        }
        [Fact]
        public async Task Delete_Id_Param()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var student = await repository.GetById("7eb89242-712a-444b-a363-61b50477c8a1");
            //This indirectly tests the Model variant as we get the Model from the id before calling it
            await repository.Delete("7eb89242-712a-444b-a363-61b50477c8a1");
            //Attempt to now get the object we just deleted, we want this to be null for this to be working
            var result = repository.GetById("7eb89242-712a-444b-a363-61b50477c8a1");
            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }
            Assert.Null(result.Result);
            await repository.Insert(student);
        }
        [Fact]
        //This tests for regression of multiple deletes crashing the app.
        public async Task Delete_Id_Param_MultipleDelete()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var deleteGuid1 = "7eb89242-712a-444b-a363-61b50477c8a1";
            var deleteGuid2 = "25204211-0ef0-488f-87e7-fd004ed3c810";
            var student1 = await repository.GetById("7eb89242-712a-444b-a363-61b50477c8a1");
            var student2 = await repository.GetById("25204211-0ef0-488f-87e7-fd004ed3c810");
            await repository.Delete(deleteGuid1);
            await repository.Delete(deleteGuid2);
            var result1 = repository.GetById(deleteGuid1);
            var result2 = repository.GetById(deleteGuid2);

            while (!result1.IsCompleted || !result2.IsCompleted)
            {
                Thread.Sleep(1);
            }
            Assert.Null(result1.Result);
            Assert.Null(result2.Result);
            await repository.Insert(student1);
            await repository.Insert(student2);
        }
        [Fact]
        public void Get_Degrees_Ret_SelectList()
        {
            var repository = MockedDatabase.GetStudentRepository();

            var result = repository.GetStudentDegrees();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<SelectList>(result);
        }
        [Fact]
        public async void UpdateStudent()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var student = new Student
            {
                FirstName = "test",
                LastName = "test",
                Id = "25204211-0ef0-488f-87e7-fd004ed3c810",
                Degree = "Test",
                GradDate = new DateTime(),
                Comments = new List<Comment>()
            };
            await repository.Update(student);
            var result = await repository.GetById("25204211-0ef0-488f-87e7-fd004ed3c810");
            Assert.Equal(student, result);
        }

        [Fact]
        public async void AddComment()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var recruiterRepo = MockedDatabase.GetRecruiterRepository();

            var comment = new Comment(await recruiterRepo.GetById(Constants.AdminRecruiter.Id), await repository.GetById("7eb89242-712a-444b-a363-61b50477c8a1"), "Test");

            await repository.AddComment(new ViewModels.StudentViewModel { Comment = comment });

            Assert.True(comment.Student.Comments.Contains(comment));
        }

        [Fact]
        public async void RemoveComment()
        {
            var repository = MockedDatabase.GetStudentRepository();

            var commentId = Guid.NewGuid().ToString();
            var comment = new Comment { Id = commentId, Student = await repository.GetById("7eb89242-712a-444b-a363-61b50477c8a1"), ActualComment = "Test", Time = new DateTime(), Recruiter = Constants.AdminRecruiter };

            await repository.AddComment(new ViewModels.StudentViewModel { Comment = comment });
            repository.RemoveComment(new ViewModels.StudentViewModel { Comment = comment });

            if (comment.Student.Comments != null)
            {
                Assert.False(comment.Student.Comments.Contains(comment));
            }
            else 
            {
                Assert.Null(comment.Student.Comments);
            }
        }
    }
}
