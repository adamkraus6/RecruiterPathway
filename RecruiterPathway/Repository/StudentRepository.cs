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
            var degreeQuery = from s in _context.Student
                              orderby s.Degree
                              select s.Degree;
            return new SelectList(degreeQuery.Distinct());
        }

        override public async ValueTask<Student> GetById(object id)
        {
            //ThenInclude from https://stackoverflow.com/a/53133582
            return await _context.Student.Include(c => c.Comments)
                .ThenInclude(r => r.Recruiter)
                .FirstOrDefaultAsync(s => s.Id == (string)id);
        }
        override public async Task<bool> Insert(Student student)
        {
            await _set.AddAsync(student);
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
            lock (_context)
            {
                //Delete all the linked objects
                var comments = _context.Comment.Where(c => c.Student.Id == student.Id);
                _context.Comment.RemoveRange(comments);
                var statuses = _context.PipelineStatus.Where(c => c.Student.Id == student.Id);
                _context.PipelineStatus.RemoveRange(statuses);
                var watches = _context.WatchList.Where(c => c.Student.Id == student.Id);
                _context.WatchList.RemoveRange(watches);
                _context.SaveChanges();
                //Delete Student MANUALLY to prevent issues with GetById
                _context.Student.Remove(student);
                _context.SaveChanges();
            }
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
            _context.Comment.Add(studentViewModel.Comment);
            Save();
        }
        public override void RemoveComment(StudentViewModel studentViewModel)
        {
            _context.Comment.Remove(studentViewModel.Comment);
            Save();
        }
    }
}
