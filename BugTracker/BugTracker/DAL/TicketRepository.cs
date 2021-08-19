using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.DAL
{
    public class TicketRepository : IGenericRepository<Ticket>
    {
        public ApplicationDbContext db;
        public TicketRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(Ticket entity)
        {
            if (entity == null)
                return;

            db.Tickets.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Ticket entity)
        {
            if (entity == null)
                return;

            db.Tickets.Remove(entity);
            db.SaveChanges();
        }

        public IQueryable<Ticket> GetCollection(Func<Ticket, bool> condition)
        {
            if (condition == null)
                return null;

            return db.Tickets.Where(condition).AsQueryable();
        }
        public IQueryable<Ticket> GetCollection()
        {
            return db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType).AsQueryable();
        }
        public Ticket GetEntity(int id)
        {
            return db.Tickets.FirstOrDefault(ticket => ticket.Id == id);
        }

        public Ticket GetEntity(Func<Ticket, bool> condition)
        {
            if (condition == null)
                return null;

            return db.Tickets.FirstOrDefault(condition);
        }

        public Ticket GetEntity(string id)
        {
            return null;
        }

        public void Update(Ticket entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void SaveChanges()
        {
            db.SaveChanges();
        }

        IEnumerable<Ticket> IGenericRepository<Ticket>.GetCollection(Func<Ticket, bool> condition)
        {
            throw new NotImplementedException();
        }
    }
}