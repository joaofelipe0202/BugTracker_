using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;


namespace BugTracker.DAL
{
    public class TicketCommentRepository : IGenericRepository<TicketComment>
    {
        public ApplicationDbContext db;

        public TicketCommentRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(TicketComment entity)
        {
            if (entity == null)
                return;

            db.TicketComments.Add(entity);
            db.SaveChanges();
        }

        public void Delete(TicketComment entity)
        {
            if (entity == null)
                return;

            db.TicketComments.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<TicketComment> GetCollection(Func<TicketComment, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketComments.Where(condition).AsEnumerable();
        }

        public TicketComment GetEntity(int id)
        {
            return db.TicketComments.FirstOrDefault(tc => tc.Id == id);
        }

        public TicketComment GetEntity(string id)
        {
            return null;
        }

        public TicketComment GetEntity(Func<TicketComment, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketComments.FirstOrDefault(condition);
        }

        public void Update(TicketComment entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}