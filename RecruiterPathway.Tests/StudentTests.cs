using Microsoft.AspNetCore.Mvc.Rendering;
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
        public void Delete_Id_Param()
        {
            var repository = MockedDatabase.GetStudentRepository();
            //This indirectly tests the Model variant as we get the Model from the id before calling it
            repository.Delete("1");
            repository.Save();
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
        public void Delete_Id_Param_MultipleDelete()
        {
            var repository = MockedDatabase.GetStudentRepository();
            var deleteGuid1 = "1";
            var deleteGuid2 = "2";
            repository.Delete(deleteGuid1);
            repository.Delete(deleteGuid2);
            repository.Save();
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
    }
}
