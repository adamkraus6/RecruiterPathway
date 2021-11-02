using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.ViewModels
{
    public class NewStudentViewModel : StudentViewModel
    {
        public Dictionary<string, Tuple<DateTime, string>> CommentView { get; set; }
    }
}
