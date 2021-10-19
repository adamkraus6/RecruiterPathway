using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;

namespace RecruiterPathway.Repository
{
    public abstract class IStudentRepository : GenericRepository<Student>, IDisposable
    {
        public IStudentRepository(DatabaseContext context) : base(context, context.Student){}
        public abstract SelectList GetStudentDegrees();
    }
}
