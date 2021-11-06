﻿using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace RecruiterPathway.Tests
{
    public class StudentTests
    {
        [Fact]
        public void GetById_Ret_Product()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var idTest = repository.GetById("1");
            while (!idTest.IsCompleted)
            {
                Thread.Sleep(1);
            }
            var result = idTest.Result;
            Assert.NotNull(result);
            Assert.IsAssignableFrom<Student>(result);
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
                Id = "9001",
                firstName = "Test",
                lastName = "Test",
                degree = "Test Test",
                gradDate = new DateTime()
            }).Result;
            Assert.IsAssignableFrom<bool>(product);
        }
        [Fact]
        public async void Delete_Id_Param()
        {
            var repository = MockedDatabase.GetStudentRepository();
            //This indirectly tests the Model variant as we get the Model from the id before calling it
            await repository.Delete("1");
            //Attempt to now get the object we just deleted, we want this to be null for this to be working
            var result = repository.GetById("1");
            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }
            Assert.Null(result.Result);
        }
        [Fact]
        //This tests for regression of multiple deletes crashing the app.
        public async void Delete_Id_Param_MultipleDelete()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var deleteGuid1 = "1";
            var deleteGuid2 = "2";
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
                firstName = "test",
                lastName = "test",
                Id = "3",
                degree = "Test",
                gradDate = new DateTime(),
                Comments = new List<Comment>()
            };
            await repository.Update(student);
            var result = await repository.GetById("3");
            Assert.Equal(student, result);
        }

        [Fact]
        public async void AddComment()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var recruiterRepo = MockedDatabase.GetRecruiterRepository();

            var comment = new Comment(await recruiterRepo.GetById(Constants.AdminRecruiter.Id), await repository.GetById("1"), "Test");

            await repository.AddComment(new ViewModels.CommentViewModel { Comment = comment });

            Assert.True(comment.Student.Comments.Contains(comment));
        }

        [Fact]
        public async void RemoveComment()
        {
            var repository = MockedDatabase.GetStudentRepository();

            var commentId = Guid.NewGuid().ToString();
            var comment = new Comment { Id = commentId, Student = await repository.GetById("1"), ActualComment = "Test", Time = new DateTime(), Recruiter = Constants.AdminRecruiter };

            await repository.AddComment(new ViewModels.CommentViewModel { Comment = comment });
            repository.RemoveComment(new ViewModels.CommentViewModel { Comment = comment });

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
