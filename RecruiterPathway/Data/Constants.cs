using RecruiterPathway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruiterPathway.Data
{
    public class Constants
    {
        public static Recruiter AdminRecruiter =
        new()
        {
            UserName = "administrator@recruiterpathway.com",
            Email = "administrator@recruiterpathway.com",
            Name = "Administrator",
            CompanyName = "Recruiter Pathway",
            PhoneNumber = "6055555555",
            Password = "P@$$w0rd",
            Id = "d2166836-4ce9-4b8e-98dd-b102c00f06f8",
            SecurityStamp = "32179209-a914-40ad-b052-bff23fe90fc4",
            WatchList = new List<Watch>(),
            PipelineStatuses = new List<PipelineStatus>()
        };
    }
}
