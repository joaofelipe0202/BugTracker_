using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.DAL
{
    public class UserRepository : IGenericRepository<ApplicationUser>
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(ApplicationDbContext context)
        {
            db = context;
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }
        public void Add(ApplicationUser entity)
        {
            if (entity == null)
                return;

            db.Users.Add(entity);
            db.SaveChanges();
        }

        public void AddToRole(string userId, string role)
        {
            if (userId == null || role == null)
                return;

            userManager.AddToRole(userId, role);
        }

        public void CreateApplicationUser(string email, string password)
        {
            if (email == null || password == null)
                return;

            if (!db.Users.Any(u => u.Email == email))
            {
                var user = new ApplicationUser() { Email = email };
                userManager.Create(user, password);
                db.SaveChanges();
            }
        }

        public void Delete(ApplicationUser entity)
        {
            if (entity == null)
                return;

            db.Users.Remove(entity);
            db.SaveChanges();
        }

        public void Delete(string id)
        {
            if (id == null)
                return;

            var user = db.Users.Find(id);

            db.Users.Remove(user);
            db.SaveChanges();
        }

        public IEnumerable<ApplicationUser> GetCollection(Func<ApplicationUser, bool> condition)
        {
            if (condition == null)
                return null;

            return db.Users.Where(condition).AsEnumerable();
        }

        public ICollection<ApplicationUser> GetCollection()
        {
            return db.Users.ToList(); 
        }

        public ApplicationUser GetEntity(int id)
        {
            return null;
        }

        public ApplicationUser GetEntity(string id)
        {
            if (id == null)
                return null;

            return db.Users.FirstOrDefault(u => u.Id == id);
        }

        public ApplicationUser GetEntity(Func<ApplicationUser, bool> condition)
        {
            if (condition == null)
                return null;

            return db.Users.FirstOrDefault(condition);
        }

        public bool IsUserInRole(string userId, string role)
        {
            if (userId == null || role == null)
                return false;

            return userManager.IsInRole(userId, role);
        }

        public bool RemoveFromRole(string userId, string role)
        {
            if (userId == null || role == null)
                return false;

            if (IsUserInRole(userId, role))
            {
                userManager.RemoveFromRoles(userId, role);
                return true;
            }
            else
                return false;
        }

        public void Update(ApplicationUser entity)
        {
            if (entity == null)
                return;

            db.Entry(entity).State = EntityState.Modified;
        }
    }
}