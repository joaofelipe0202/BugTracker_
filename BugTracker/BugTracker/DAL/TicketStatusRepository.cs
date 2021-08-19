using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.DAL
{
    public class TicketStatusRepository : IGenericRepository<TicketStatus>
    {
        public ApplicationDbContext db;

        public TicketStatusRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(TicketStatus entity)
        {
            if (entity == null)
                return;

            db.TicketStatuses.Add(entity);
            db.SaveChanges();
        }

        public void Delete(TicketStatus entity)
        {
            if (entity == null)
                return;

            db.TicketStatuses.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<TicketStatus> GetCollection(Func<TicketStatus, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketStatuses.Where(condition).AsEnumerable();
        }
        public IEnumerable<TicketStatus> GetCollection()
        {
            return db.TicketStatuses.AsEnumerable();
        }

        public TicketStatus GetEntity(int id)
        {
            return db.TicketStatuses.FirstOrDefault(ts => ts.Id == id);
        }

        public TicketStatus GetEntity(string id)
        {
            return db.TicketStatuses.Find(id);
        }

        public TicketStatus GetEntity(Func<TicketStatus, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketStatuses.FirstOrDefault(condition);
        }


        public void Update(TicketStatus entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}