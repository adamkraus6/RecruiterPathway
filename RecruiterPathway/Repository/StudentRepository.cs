using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Threading;
using RecruiterPathway.ViewModels;
using Microsoft.EntityFrameworkCore;

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
        override async public Task Delete(object id)
        {
            var student = await GetById(id);
            if (student.comments != null)
            {
                student.comments.Clear();
            }
            await base.Delete(id);
        }
        public override async Task AddComment(CommentViewModel view)
        {
            //context.Student.Include(p => p.comments).FirstOrDefault();
            var student = await GetById(view.Comment.StudentId);
            if (student.comments == null)
            {
                student.comments = new List<Comment>();
            }
            context.Comment.Add(view.Comment);
            Save();
        }
        public override async Task RemoveComment(CommentViewModel view)
        {
            var student = view.Comment.Student;
            if (student.comments == null)
            {
                return;
            }
            context.Comment.Remove(view.Comment);
            Save();
        }

        override public ICollection<Comment> GetCommentsForStudent(Student student)
        {
            return context.Comment.Where(s => s.Student == student).ToList();
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
