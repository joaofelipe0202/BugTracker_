using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.DAL
{
    public class TicketAttachmentRepository : IGenericRepository<TicketAttachment>
    {
        public ApplicationDbContext db;

        public TicketAttachmentRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(TicketAttachment entity)
        {
            if (entity == null)
                return;

            db.TicketAttachments.Add(entity);
            db.SaveChanges();
        }

        public void Delete(TicketAttachment entity)
        {
            if (entity == null)
                return;

            db.TicketAttachments.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<TicketAttachment> GetCollection(Func<TicketAttachment, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketAttachments.Where(condition).AsEnumerable();
        }

        public TicketAttachment GetEntity(int id)
        {
            return db.TicketAttachments.FirstOrDefault(ta => ta.Id == id);
        }

        public TicketAttachment GetEntity(string id)
        {
            return null;
        }

        public TicketAttachment GetEntity(Func<TicketAttachment, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketAttachments.FirstOrDefault(condition);
        }

        public void Update(TicketAttachment entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}