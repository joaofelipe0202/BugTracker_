using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;


namespace BugTracker.DAL
{
    public class TicketHistoryRepository : IGenericRepository<TicketHistory>
    {
        public ApplicationDbContext db;

        public TicketHistoryRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(TicketHistory entity)
        {
            if (entity == null)
                return;

            db.TicketHistories.Add(entity);
            db.SaveChanges();
        }

        public void Delete(TicketHistory entity)
        {
            if (entity == null)
                return;

            db.TicketHistories.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<TicketHistory> GetCollection(Func<TicketHistory, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketHistories.Where(condition).AsEnumerable();
        }

        public TicketHistory GetEntity(int id)
        {
            return db.TicketHistories.FirstOrDefault(th => th.Id == id);
        }

        public TicketHistory GetEntity(string id)
        {
            return null;
        }

        public TicketHistory GetEntity(Func<TicketHistory, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketHistories.FirstOrDefault(condition);
        }

        public void Update(TicketHistory entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}