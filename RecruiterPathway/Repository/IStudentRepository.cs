using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using RecruiterPathway.ViewModels;

namespace RecruiterPathway.Repository
{
    public abstract class IStudentRepository : GenericRepository<Student>, IDisposable
    {
        public IStudentRepository(DatabaseContext context) : base(context, context.Student){}
        public abstract SelectList GetStudentDegrees();
        public abstract void AddComment(CommentViewModel view);
        public abstract void RemoveComment(CommentViewModel view);
        //public abstract void UpdateComment(CommentViewModel view);
    }
}
