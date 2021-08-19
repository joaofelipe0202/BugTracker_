using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.DAL
{
    public class RoleRepository : IGenericRepository<IdentityRole>
    {
        public ApplicationDbContext db;
        public RoleManager<IdentityRole> roleManager;

        public RoleRepository(ApplicationDbContext context)
        {
            db = context;
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public void Add(IdentityRole entity)
        {
            if (entity == null)
                return;

            roleManager.Create(entity);
            db.SaveChanges();
        }

        public void Delete(IdentityRole entity)
        {
            if (entity == null)
                return;

            roleManager.Delete(entity);
            db.SaveChanges();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IdentityRole> GetCollection(Func<IdentityRole, bool> condition)
        {
            if (condition == null)
                return null;

            return roleManager.Roles.Where(condition).ToList();
        }

        public ICollection<IdentityRole> GetCollection()
        {
            return roleManager.Roles.ToList();
        }

        public IdentityRole GetEntity(int id)
        {
            return null;
        }

        public IdentityRole GetEntity(string id)
        {
            if (id == null)
                return null;

            return roleManager.FindById(id);
        }

        public IdentityRole GetEntity(Func<IdentityRole, bool> condition)
        {
            if (condition == null)
                return null;

            return roleManager.Roles.FirstOrDefault(condition);
        }

        public void Update(IdentityRole entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}