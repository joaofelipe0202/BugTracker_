using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public TicketStatus() { }
        public TicketStatus(string Name) 
        {
            Tickets = new HashSet<Ticket>();
            this.Name = Name;
        }

    }
    public class TicketPriority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        public TicketPriority() { }
        public TicketPriority(string Name)
        {
            Tickets = new HashSet<Ticket>();

            this.Name = Name;
        }
    }
    public class TicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        public TicketType() { }
        public TicketType(string Name)
        {
            Tickets = new HashSet<Ticket>();
            this.Name = Name;
        }
    }

}