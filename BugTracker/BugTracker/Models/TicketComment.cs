using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
        public TicketComment() { }
        public TicketComment(string comment) 
        {
            this.Comment = comment;
            this.Created = DateTime.Now;
        }
        public TicketComment(string comment, int TicketId, string UserId)
        {
            this.Comment = comment;
            this.Created = DateTime.Now;
            this.TicketId = TicketId;
            this.UserId = UserId;
        }

    }
}