using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.DAL
{
    public class TicketPriorityRepository : IGenericRepository<TicketPriority>
    {
        public ApplicationDbContext db;

        public TicketPriorityRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(TicketPriority entity)
        {
            if (entity == null)
                return;

            db.TicketPriorities.Add(entity);
            db.SaveChanges();
        }

        public void Delete(TicketPriority entity)
        {
            if (entity == null)
                return;

            db.TicketPriorities.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<TicketPriority> GetCollection(Func<TicketPriority, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketPriorities.AsEnumerable();
        }
        public IEnumerable<TicketPriority> GetCollection()
        {
            return db.TicketPriorities.AsEnumerable();
        }

        public TicketPriority GetEntity(int id)
        {
            return db.TicketPriorities.FirstOrDefault(tp => tp.Id == id);
        }

        public TicketPriority GetEntity(string id)
        {
            return null;
        }

        public TicketPriority GetEntity(Func<TicketPriority, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketPriorities.FirstOrDefault(condition);
        }

        public void Update(TicketPriority entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges(); ;
        }
    }
}