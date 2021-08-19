using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.BL;
using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using PagedList;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly MembershipService MembershipService;
        private readonly ProjectService ProjectService;

        public ProjectsController()
        {
            this.db = new ApplicationDbContext();
            this.MembershipService = new MembershipService(db);
            this.ProjectService = new ProjectService(db);
        }

        [HttpGet]

        public ActionResult RemoveUserFromProject(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.UserId = new SelectList(MembershipService.GetUsersOfThisProject(id), "Id", "UserName");
            return View();
        }

        [HttpPost]
        public ActionResult RemoveUserFromProject(int? id, string userId)
        {
            if (id == null || string.IsNullOrEmpty(userId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var result = ProjectService.RemoveUserFromProject((int)id, userId);
            if (result)
                return RedirectToAction("Dashboard", "Users");

            ViewBag.UserId = new SelectList(MembershipService.GetUsersOfThisProject(id), "Id", "UserName");
            return View();
        }

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AllProjects(int? page)
        {
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            string UserId = User.Identity.GetUserId();
            return View(ProjectService.GetAllProjects(UserId).ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Developer, Submitter")]
        public ActionResult GetAssignedProjects()
        {
            string UserId = User.Identity.GetUserId();
            var Projects = ProjectService.GetAssignedProjectsOfDeveloperAndSubmitter(UserId);
            return View(Projects);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            string UserId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = ProjectService.GetAllProjects(UserId).FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();

            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "Name");
            ViewBag.IsAdmin = MembershipService.IsAuthorizedAsAdmin(userId);
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project, string UserId)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                if (MembershipService.IsAuthorizedAsAdmin(userId))
                {
                    ProjectService.AssignUserToProject(project.Id, UserId);
                }
                else if (MembershipService.IsAuthorizedAsProjectManager(userId))
                {
                    ProjectService.AssignUserToProject(project.Id, userId);
                }
                ProjectService.AddProject(project);
                return RedirectToAction("Dashboard", "Users");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            string UserId = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = ProjectService.GetAllProjects(UserId).FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Delete(int? id)
        {
            string UserId = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = ProjectService.GetAllProjects(UserId).FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("AllProjects");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignUserToAProject()
        {
            string UserId = User.Identity.GetUserId();
            ViewBag.UserId = new SelectList(MembershipService.GetDevelopers(), "Id", "UserName");
            ViewBag.ProjectId = new SelectList(ProjectService.GetAllProjects(UserId), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult AssignUserToAProject(string UserId, int ProjectId)
        {
            bool result = ProjectService.AssignUserToProject(ProjectId, UserId);
            if (result)
            {
                RedirectToAction("Details", "Projects", new { Id = ProjectId });
            }
            ViewBag.UserId = new SelectList(MembershipService.GetDevelopers(), "Id", "UserName");
            ViewBag.ProjectId = new SelectList(ProjectService.GetAllProjects(UserId), "Id", "Name");
            return RedirectToAction("Details", "Projects", new { Id = ProjectId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
