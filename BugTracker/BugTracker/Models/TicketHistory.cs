using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool Changed { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
        public TicketHistory() { }
        public TicketHistory(string property, string oldValue, string newValue)
        {
            this.Property = property;
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }

        public TicketHistory(string property, string oldValue, string newValue, int TicketId, string UserId)
        {
            this.Property = property;
            this.NewValue = newValue;
            this.OldValue = oldValue;
            this.TicketId = TicketId;
            this.UserId = UserId;
        }

    }
}