using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser user { get; set; }
        public TicketNotification() { }
        public TicketNotification( int TicketId,string userId) 
        {
            this.TicketId = TicketId;
            this.UserId = userId;
        }
    }
}