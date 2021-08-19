using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.DAL
{
    public class TicketNotificationRepository : IGenericRepository<TicketNotification>
    {
        public ApplicationDbContext db;

        public TicketNotificationRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(TicketNotification entity)
        {
            if (entity == null)
                return;

            db.TicketNotifications.Add(entity);
            db.SaveChanges();
        }

        public void Delete(TicketNotification entity)
        {
            if (entity == null)
                return;

            db.TicketNotifications.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<TicketNotification> GetCollection(Func<TicketNotification, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketNotifications.Where(condition).AsEnumerable();
        }

        public TicketNotification GetEntity(int id)
        {
            return db.TicketNotifications.FirstOrDefault(tn => tn.Id == id);
        }

        public TicketNotification GetEntity(string id)
        {
            return null;
        }

        public TicketNotification GetEntity(Func<TicketNotification, bool> condition)
        {
            return db.TicketNotifications.FirstOrDefault(condition);
        }

        public void Update(TicketNotification entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}