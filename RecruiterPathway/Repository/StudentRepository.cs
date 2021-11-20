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
            _context.Comment.Add(studentViewModel.Comment);
            Save();
        }
        public override void RemoveComment(StudentViewModel studentViewModel)
        {
            _context.Comment.Remove(studentViewModel.Comment);
            Save();
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
