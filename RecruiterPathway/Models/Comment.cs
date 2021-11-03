using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string RecruiterId { get; set; }
        public DateTime Time { get; set; }
        public string ActualComment { get; set; }
    }
}
