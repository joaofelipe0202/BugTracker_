using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }
        public virtual Project Project { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }


        public Ticket() { }
        public Ticket(string Title, string Description)
        {
            this.Title = Title;
            this.Description = Description;
            this.Created = DateTime.Now;
            TicketNotifications = new HashSet<TicketNotification>();
            TicketHistories = new HashSet<TicketHistory>();
            TicketComments = new HashSet<TicketComment>();
            TicketAttachments = new HashSet<TicketAttachment>();
        }
        public Ticket(string Title, string Description, int projectId , int TicketTypeId , int TicketPriorityId , int TicketStatusId,string  OwnerUserId, string AssignedToUserId) 
        { 
            this.Title = Title;
            this.Description = Description;
            this.ProjectId = projectId;
            this.TicketPriorityId = TicketPriorityId;
            this.TicketTypeId = TicketTypeId;
            this.TicketStatusId = TicketStatusId;
            this.OwnerUserId = OwnerUserId;
            this.AssignedToUserId = AssignedToUserId;
            this.Created = DateTime.Now;
            TicketNotifications = new HashSet<TicketNotification>();
            TicketHistories = new HashSet<TicketHistory>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketComments = new HashSet<TicketComment>();
        }
    }
}