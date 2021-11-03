using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using System;
using RecruiterPathway.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RecruiterPathway.Repository
{
    public abstract class IStudentRepository : GenericRepository<Student>, IDisposable
    {
        public IStudentRepository(DatabaseContext context) : base(context, context.Student){}
        public abstract SelectList GetStudentDegrees();
        public abstract Task AddComment(CommentViewModel view);
        public abstract Task RemoveComment(CommentViewModel view);

        public abstract ICollection<Comment> GetCommentsForStudent(Student student);
        //public abstract void UpdateComment(CommentViewModel view);
    }
}
