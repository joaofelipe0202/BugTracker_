using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.DAL
{
    public class TicketTypeRepository : IGenericRepository<TicketType>
    {
        public ApplicationDbContext db;

        public TicketTypeRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(TicketType entity)
        {
            if (entity == null)
                return;

            db.TicketTypes.Add(entity);
            db.SaveChanges();
        }

        public void Delete(TicketType entity)
        {
            if (entity == null)
                return;

            db.TicketTypes.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<TicketType> GetCollection(Func<TicketType, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketTypes.Where(condition).AsEnumerable();
        }
        public IEnumerable<TicketType> GetCollection()
        {
            return db.TicketTypes.AsEnumerable();
        }

        public TicketType GetEntity(int id)
        {
            return db.TicketTypes.FirstOrDefault(tt => tt.Id == id);
        }

        public TicketType GetEntity(string id)
        {
            return null;
        }

        public TicketType GetEntity(Func<TicketType, bool> condition)
        {
            if (condition == null)
                return null;

            return db.TicketTypes.FirstOrDefault(condition);
        }

        public void Update(TicketType entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}