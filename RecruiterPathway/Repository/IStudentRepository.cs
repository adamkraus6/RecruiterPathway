using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RecruiterPathway.ViewModels;

namespace RecruiterPathway.Repository
{
    public abstract class IStudentRepository : GenericRepository<Student>
    {
        protected IStudentRepository(DatabaseContext context) : base(context, context.Student){}
        public abstract SelectList GetStudentDegrees();
        public abstract Task AddComment(StudentViewModel studentViewModel);
        public abstract void RemoveComment(StudentViewModel studentViewModel);

        public abstract Task<IEnumerable<Student>> GetAll(StudentViewModel studentViewModel);
    }
}
