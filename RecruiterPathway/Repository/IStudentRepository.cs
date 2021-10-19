using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public interface IStudentRepository : IDisposable
    {
        IEnumerable<Student> GetStudents();
        SelectList GetStudentDegrees();
        Task<Student> GetStudentById(int id);
        void InsertStudent(Student student);
        void DeleteStudent(int id);
        void UpdateStudent(Student student);
        void Save();

    }
}
