using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectUser
    {
        public int Id { get; set; }
        public string TestFlag { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public virtual ApplicationUser user { get; set; }
        public virtual Project Project { get; set; }
        public ProjectUser() { }
        public ProjectUser(int projectId, string UserId)
        {
            this.ProjectId = projectId;
            this.UserId = UserId;
        }
    }
}