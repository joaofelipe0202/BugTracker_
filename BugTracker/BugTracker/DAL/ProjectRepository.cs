using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BugTracker.Models;

namespace BugTracker.DAL
{
    public class ProjectRepository : IGenericRepository<Project>
    {
        public ApplicationDbContext db;

        public ProjectRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Add(Project entity)
        {
            if (entity == null)
                return;

            db.Projects.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Project entity)
        {
            if (entity == null)
                return;

            db.Projects.Remove(entity);
            db.SaveChanges();
        }

        public IEnumerable<Project> GetCollection(Func<Project, bool> condition)
        {
            if (condition == null)
                return null;

            return db.Projects.Where(condition).AsEnumerable();
        }
        public ICollection<Project> GetCollection()
        {
            return db.Projects.ToList();
        }
        
        public Project GetEntity(int id)
        {
            return db.Projects.FirstOrDefault(p => p.Id == id);
        }

        public Project GetEntity(string id)
        {
            return null;
        }

        public Project GetEntity(Func<Project, bool> condition)
        {
            if (condition == null)
                return null;

            return db.Projects.FirstOrDefault(condition);
        }

        public void Update(Project entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}