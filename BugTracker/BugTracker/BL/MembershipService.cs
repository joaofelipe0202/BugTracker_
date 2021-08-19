using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.BL
{
    public class MembershipService
    {
        private readonly RoleRepository roleRepo;
        private readonly UserRepository userRepo;
        public MembershipService(ApplicationDbContext context)
        {
            this.roleRepo = new RoleRepository(context);
            this.userRepo = new UserRepository(context);
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public IEnumerable<IdentityRole> GetRoles()
        {
            return roleRepo.GetCollection();
        }

        [Authorize(Roles = "Admin")]
        public void CreateUser(string email, string password)
        {
            userRepo.CreateApplicationUser(email, password);
        }

        [Authorize(Roles = "Admin")]
        public bool CheckIfUserHasARole(string userId, string roleName)
        {
            return userRepo.IsUserInRole(userId, roleName);
        }

        [Authorize(Roles = "Admin")]
        public bool AddUserToRole(string userId, string roleName)
        {
            if (CheckIfUserHasARole(userId, roleName))
            {
                return false;
            }
            else
            {
                userRepo.AddToRole(userId, roleName);
                return true;
            }
        }

        [Authorize(Roles = "Admin")]
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            return userRepo.RemoveFromRole(userId, roleName);
        }

        [Authorize]
        public bool IsAuthorizedAsAdmin(string userId)
        {
            if (userId != null && userRepo.IsUserInRole(userId, "Admin"))
            {
                return true;
            }
            return false;
        }

        [Authorize]
        public bool IsAuthorizedAsProjectManager(string userId)
        {
            if (userId != null && userRepo.IsUserInRole(userId, "Project Manager"))
            {
                return true;
            }
            return false;
        }

        [Authorize]
        public bool IsAuthorizedAsDeveloper(string userId)
        {
            if (userId != null && userRepo.IsUserInRole(userId, "Developer"))
            {
                return true;
            }
            return false;
        }

        [Authorize]
        public bool IsAuthorizedAsSubmitter(string userId)
        {
            if (userId != null && userRepo.IsUserInRole(userId, "Submitter"))
            {
                return true;
            }
            return false;
        }
        [Authorize(Roles = "Admin, Project Manager")]
        public ICollection<ApplicationUser> GetUsers()
        {
            return userRepo.GetCollection();
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ICollection<ApplicationUser> GetUsersOfThisProject(int? id)
        {
            if (id == null)
                return null;
            return userRepo.GetCollection().Where(u => u.ProjectUsers.Any(pu => pu.ProjectId == id)).ToList();
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public IEnumerable<ApplicationUser> GetDevelopers()
        {
            return userRepo.GetCollection(u => IsAuthorizedAsDeveloper(u.Id));
        }
        [Authorize]
        public void UpdateUserProfile(string userId)
        {
            if (userId == null)
                return;

            var user = userRepo.GetEntity(userId);
            if (user == null)
            {
                return;
            }
            userRepo.Update(user);
        }

        [Authorize]
        public void DeleteUser(string userId)
        {
            if (userId == null)
                return;

            var user = userRepo.GetEntity(userId);
            if (user == null)
            {
                return;
            }
            userRepo.Delete(userId);
        }
    }
}