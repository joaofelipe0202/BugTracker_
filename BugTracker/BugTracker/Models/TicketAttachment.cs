using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string FileUrl { get; set; }
        public virtual Ticket Ticket { get; set; }
        public ApplicationUser User { get; set; }
        public TicketAttachment() { }
        public TicketAttachment(string filePath, string Description) 
        {
            this.FilePath = filePath;
            this.Description = Description;
            this.Created = DateTime.Now;
        }
        public TicketAttachment(string filePath, string Description, int TicketId, string userId, string FileUrl)
        { 
            this.FilePath = filePath;
            this.Description = Description;
            this.Created = DateTime.Now;
            this.TicketId = TicketId;
            this.UserId = userId;
            this.FileUrl = FileUrl; 
        }
    }
}