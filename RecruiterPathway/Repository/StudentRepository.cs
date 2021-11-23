using System.Linq;
using System.Threading.Tasks;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.ViewModels;
using RecruiterPathway.Data;
using Microsoft.EntityFrameworkCore;

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
            Student student;
            lock (_context)
            {
                student =  _context.Student.Include(c => c.Comments)
                    .ThenInclude(r => r.Recruiter)
                    .FirstOrDefault(s => s.Id == (string)id);
            }
            return student;
        }
        override public async Task<bool> Insert(Student student)
        {
            lock (_context) lock (_set)
                {
                    _set.Add(student);
                }
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
            lock (_context)
            {
                //Perform a duplication check cause even if it's a 1 in a million chance, we could have a concurrency issue
                var comment = _context.Comment.Where(c => c.Id == studentViewModel.Comment.Id).FirstOrDefault();
                if (comment != null) 
                {
                    studentViewModel.Comment.Id = Guid.NewGuid().ToString();
                }
                _context.Comment.Add(studentViewModel.Comment);
                _context.SaveChanges();
            }
        }
        public override void RemoveComment(StudentViewModel studentViewModel)
        {
            lock (_context)
            {
                var comment = _context.Comment.Where(c => c.Id == studentViewModel.Comment.Id).FirstOrDefault();
                if (comment == null)
                {
                    return;
                }
                _context.Comment.Remove(studentViewModel.Comment);
                _context.SaveChanges();
            }
        }

        override public async Task<IEnumerable<Student>> GetAll(StudentViewModel studentViewModel)
        {
            IEnumerable<Student> students = await GetAll();

            if (!string.IsNullOrEmpty(studentViewModel.SearchFirstName))
            {
                students = students.Where(st => st.FirstName.Contains(studentViewModel.SearchFirstName));
            }

            if (!string.IsNullOrEmpty(studentViewModel.SearchLastName))
            {
                students = students.Where(st => st.LastName.Contains(studentViewModel.SearchLastName));
            }

            if (!string.IsNullOrEmpty(studentViewModel.StudentDegree))
            {
                students = students.Where(st => st.Degree == studentViewModel.StudentDegree);
            }

            if (DateTime.MinValue != studentViewModel.GradDateStart && DateTime.MinValue != studentViewModel.GradDateEnd)
            {
                students = students.Where(st => studentViewModel.GradDateStart.CompareTo(st.GradDate) < 0 && studentViewModel.GradDateEnd.CompareTo(st.GradDate) >= 0);
            }

            switch(studentViewModel.SortBy)
            {
                default:
                    break;
                case StudentViewModel.SortOptions.NONE:
                    break;
                case StudentViewModel.SortOptions.FIRST:
                    students = students.OrderBy(s => s.FirstName);
                    break;
                case StudentViewModel.SortOptions.LAST:
                    students = students.OrderBy(s => s.LastName);
                    break;
                case StudentViewModel.SortOptions.DEG:
                    students = students.OrderBy(s => s.Degree);
                    break;
                case StudentViewModel.SortOptions.GRAD:
                    students = students.OrderBy(s => s.GradDate);
                    break;
            }

            return students;
        }
    }
}
