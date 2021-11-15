using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.ViewModels;

namespace RecruiterPathway.Repository
{
    public class StudentRepository : IStudentRepository
    {
        public StudentRepository(DatabaseContext context) : base(context) { }

        override public SelectList GetStudentDegrees()
        {
            var degreeQuery = from s in context.Student
                              orderby s.Degree
                              select s.Degree;
            return new SelectList(degreeQuery.Distinct());
        }

        override public async ValueTask<Student> GetById(object id)
        {
            //ThenInclude from https://stackoverflow.com/a/53133582
            return await context.Student.Include(c => c.Comments)
                .ThenInclude(r => r.Recruiter)
                .FirstOrDefaultAsync(s => s.Id == (string)id);
        }
        override public async Task<bool> Insert(Student student)
        {
            await set.AddAsync(student);
            Save();
            return true;

        }
        override async public Task Delete(object id)
        {
            var student = await GetById(id);
            if (student == null)
            {
                return;
            }
            if (student.Comments != null)
            {
                student.Comments.Clear();
            }
            await base.Delete(id);
        }
        public override async Task AddComment(StudentViewModel studentViewModel)
        {
            var student = studentViewModel.Student;
            if (student == null)
            {
                return;
            }
            if (student.Comments == null)
            {
                student.Comments = new List<Comment>();
            }
            context.Comment.Add(studentViewModel.Comment);
            Save();
        }
        public override void RemoveComment(StudentViewModel studentViewModel)
        {
            context.Comment.Remove(studentViewModel.Comment);
            Save();
        }
    }
}
