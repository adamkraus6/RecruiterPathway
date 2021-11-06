using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var product = repository.Insert(Constants.AdminRecruiter).Result;
            Assert.IsAssignableFrom<bool>(product);
        }
        [Fact]
        public async void Delete_Id_Param()
        {
            var repository = MockedDatabase.GetRecruiterRepository();
            //This indirectly tests the Model variant as we get the Model from the id before calling it
            var deleteGuid = MockedDatabase.GetRandomGuid();
            await repository.Delete(deleteGuid);

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
        public async void Delete_Id_Param_MultipleDelete()
        {
            var repository = MockedDatabase.GetRecruiterRepository();
            var deleteGuid1 = MockedDatabase.GetRandomGuid();
            var deleteGuid2 = MockedDatabase.GetRandomGuid();
            var waiting = repository.Delete(deleteGuid1);
            while (!waiting.IsCompleted)
            {
                Thread.Sleep(1);
            }
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
        [Fact]
        public async void SetPipelineStatus()
        {
            var repository = MockedDatabase.GetRecruiterRepository();

            var result = await repository.SetPipelineStatus(Constants.AdminRecruiter.Id, "1", "inprogress");
            Assert.True(result);

            var recruiter = await repository.GetById(Constants.AdminRecruiter.Id);
            var pipelineStatus = recruiter.PipelineStatuses.FirstOrDefault(s => s.Student.Id == "1");
            Assert.IsAssignableFrom<PipelineStatus>(pipelineStatus);
        }
        [Fact]
        public async void AddWatch()
        {
            var repository = MockedDatabase.GetRecruiterRepository();
            var studentRepo = MockedDatabase.GetStudentRepository();

            Student student = await studentRepo.GetById("1");
            await repository.AddWatch(Constants.AdminRecruiter.Id, student);
            var recruiter = await repository.GetById(Constants.AdminRecruiter.Id);

            Assert.True(recruiter.WatchList.Contains(recruiter.WatchList.FirstOrDefault(w => w.Student == student)));
        }
        [Fact]
        public async void RemoveWatch()
        {
            var repository = MockedDatabase.GetRecruiterRepository();
            var studentRepo = MockedDatabase.GetStudentRepository();

            Student student = await studentRepo.GetById("1");
            await repository.AddWatch(Constants.AdminRecruiter.Id, student);
            var recruiter = await repository.GetById(Constants.AdminRecruiter.Id);

            if (recruiter.WatchList == null)
            {
                Assert.Null(recruiter.WatchList);
            }
            else
            {
                Assert.False(recruiter.WatchList.FirstOrDefault(w => w.Student == student) != null);
            }
        }
    }
}
