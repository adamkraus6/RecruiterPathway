using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Threading;

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
                context.Student.Add(student);
                return true;
            }
            return false;
        }
        override async public void Delete(object id) 
        {
            if (exists(id))
            {
                Task<Student> student = context.Student.FindAsync(id).AsTask();
                while (!student.IsCompleted) 
                {
                    Thread.Sleep(1);
                }
                context.Student.Remove(student.Result);
            }
        }
        //TODO: FINISH ME
        private bool IsValid(Student student)
        {
            return true;
        }

        //TODO: FINISH ME
        private bool exists(object id)
        {
            return context.Student.Any(e => e.Id.Equals(id));
        }
    }
}
