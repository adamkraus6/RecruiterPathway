using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace RecruiterPathway.Tests
{
    public class RecruiterTests
    {
        [Fact]
        public void GetAll_Ret_Product()
        {
            var repository = MockedDatabase.GetRecruiterRepository();
            var product = repository.GetAll().Result;
            Assert.NotNull(product);
            Assert.IsAssignableFrom<List<Recruiter>>(product);
        }
        [Fact]
        public void Insert_Ret_Product()
        {
            var repository = MockedDatabase.GetRecruiterRepository();
            var product = repository.Insert(new Recruiter
            {
                UserName = "administrator@recruiterpathway.com",
                Email = "administrator@recruiterpathway.com",
                Name = "Administrator",
                CompanyName = "Recruiter Pathway",
                PhoneNumber = "6055555555",
                Password = "P@$$w0rd",
                Id = "d2166836-4ce9-4b8e-98dd-b102c00f06f8",
                SecurityStamp = "32179209-a914-40ad-b052-bff23fe90fc4"
            }).Result;
            Assert.IsAssignableFrom<bool>(product);
        }
        [Fact]
        public void Delete_Id_Param()
        {
            var repository = MockedDatabase.GetRecruiterRepository();
            //This indirectly tests the Model variant as we get the Model from the id before calling it
            var deleteGuid = MockedDatabase.GetRandomGuid();
            repository.Delete(deleteGuid);

            //Attempt to now get the object we just deleted, we want this to be null for this to be working
            var result = repository.GetById(deleteGuid);
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
            var repository = MockedDatabase.GetRecruiterRepository();
            var deleteGuid1 = MockedDatabase.GetRandomGuid();
            var deleteGuid2 = MockedDatabase.GetRandomGuid();
            repository.Delete(deleteGuid1);
            repository.Delete(deleteGuid2);

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
        public void SignInRecruiter_Valid()
        {
            var repository = MockedDatabase.GetRecruiterRepository();

            var randomRecruiter = repository.GetById(MockedDatabase.GetRandomGuid());
            while (!randomRecruiter.IsCompleted)
            {
                Thread.Sleep(1);
            }
            var result = repository.SignInRecruiter(randomRecruiter.Result);

            Assert.NotNull(result);
        }

    }
}
