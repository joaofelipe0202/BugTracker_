using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectManagerViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectManagerName { get; set; }
    }

    public class TicketsViewModel
    {
        public int TicketId { get; set; }
        public string TicketName { get; set; }
        public string ProjectName { get; set; }
        public TicketType TicketType { get; set; }
        public TicketPriority TicketPriority { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string TicketDescription { get; set; }
        public string OwnerUserName { get; set; }
        public string AssignedToUserName { get; set; }

        public TicketsViewModel(int Id, string Name, string Desc, string OwnerName, string AssignedUserName, DateTime created, DateTime Updated, string ProjectName, TicketType ticketType, TicketPriority ticketPriority , TicketStatus ticketStatus)
        {
            this.TicketName = Name;
            this.TicketId = Id;
            this.TicketDescription = Desc;
            this.OwnerUserName = OwnerName;
            this.AssignedToUserName = AssignedUserName;
            this.CreatedDate = created;
            this.UpdatedDate = Updated;
            this.ProjectName = ProjectName;
            this.TicketType = ticketType;
            this.TicketStatus = ticketStatus;
            this.TicketPriority = ticketPriority;
        }

        public TicketsViewModel()
        {
        }
    }
}