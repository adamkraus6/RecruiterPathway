using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.ViewModels
{
    public class NewStudentViewModel : StudentViewModel
    {
        public List<Tuple<string, DateTime, string>> CommentView { get; set; }
    }
}
