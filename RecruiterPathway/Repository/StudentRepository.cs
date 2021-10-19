using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecruiterPathway.Repository
{
    public class StudentRepository : IStudentRepository, IDisposable
    {
        private DatabaseContext context;

        public StudentRepository(DatabaseContext context)
        {
            this.context = context;
        }

        IEnumerable<Student> IStudentRepository.GetStudents() 
        {
            return context.Student.ToList();
        }

        SelectList IStudentRepository.GetStudentDegrees()
        {
            var degreeQuery = from s in context.Student
                              orderby s.degree
                              select s.degree;
            return new SelectList(degreeQuery.Distinct());
        }

        async Task<Student> IStudentRepository.GetStudentById(int id) 
        {
            return await context.Student.FindAsync(id).AsTask();
        }
        void IStudentRepository.InsertStudent(Student student) 
        {
            if(IsValid(student))
                context.Student.Add(student);
        }
        async void IStudentRepository.DeleteStudent(int id) 
        {
            if (exists(id))
            {
                Student student = (await context.Student.FindAsync(id).AsTask());
                context.Student.Remove(student);
            }
        }
        void IStudentRepository.UpdateStudent(Student student) 
        {
            context.Entry(student).State = EntityState.Modified;
        }
        void IStudentRepository.Save() 
        {
            context.SaveChanges();
        }

        //TODO: FINISH ME
        private bool IsValid(Student student)
        {
            return true;
        }

        //TODO: FINISH ME
        private bool exists(int id)
        {
            return context.Student.Any(e => e.Id == id);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
