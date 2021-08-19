using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.DAL
{
    public class ProjectUserRepository : IGenericRepository<ProjectUser>
    {
        public ApplicationDbContext db;

        public ProjectUserRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(ProjectUser entity)
        {
            db.ProjectUsers.Add(entity);
            db.SaveChanges();
        }

        public void Delete(ProjectUser entity)
        {
            db.ProjectUsers.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<ProjectUser> GetCollection(Func<ProjectUser, bool> condition)
        {
            return db.ProjectUsers.AsEnumerable();
        }

        public ProjectUser GetEntity(int id)
        {
            return db.ProjectUsers.Find(id);
        }

        public ProjectUser GetEntity(Func<ProjectUser, bool> condition)
        {
            return db.ProjectUsers.FirstOrDefault(condition);
        }

        public ProjectUser GetEntity(string id)
        {
            return db.ProjectUsers.Find(id);
        }

        public void Update(ProjectUser entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}