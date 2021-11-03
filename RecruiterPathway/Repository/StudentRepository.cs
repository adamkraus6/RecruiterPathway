using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Threading;
using RecruiterPathway.ViewModels;

namespace RecruiterPathway.Repository
{
    public class StudentRepository : IStudentRepository, IDisposable
    {
        public StudentRepository(DatabaseContext context) : base(context) { }

        override public SelectList GetStudentDegrees()
        {
            var degreeQuery = from s in context.Student
                              orderby s.degree
                              select s.degree;
            return new SelectList(degreeQuery.Distinct());
        }

        override public async Task<bool> Insert(Student student)
        {
            if (IsValid(student))
            {

                await set.AddAsync(student);

                //context.Student.Add(student);
                Save();
                Console.WriteLine("called insert(student)");
                return true;
            }
            return false;
        }
        override public void Save()
        {
            context.Student = set;
            base.Save();
        }
        override async public Task Delete(object id)
        {
            var student = await GetById(id);
            if (student.comments != null)
            {
                student.comments.Clear();
            }
            await base.Delete(id);
        }
        public override void AddComment(CommentViewModel view)
        {
            var student = view.Student;
            if (student.comments == null)
            {
                student.comments = new List<Comment>();
            }
            student.comments.Add(new Comment(view.Recruiter.Id, DateTime.UtcNow, view.Comment));
            Update(student);
        }
        public override void RemoveComment(CommentViewModel view)
        {
            
        }
        //TODO: FINISH ME
        private bool IsValid(Student student)
        {
            return true;
        }

        //TODO: FINISH ME
        private bool exists(object id)
        {
            return set.Any(e => e.Id.Equals(id));
        }
    }
}
