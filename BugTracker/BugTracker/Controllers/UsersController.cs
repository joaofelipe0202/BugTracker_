using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.BL;
using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace BugTracker.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly MembershipService MembershipService;
        private readonly ProjectService ProjectService;
        private readonly TicketService TicketService;

        public UsersController()
        {
            this.db = new ApplicationDbContext();
            this.MembershipService = new MembershipService(db);
            this.ProjectService = new ProjectService(db);
            this.TicketService = new TicketService(db);
        }

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DashBoard(int? page, string SortBy, string FilterBy, bool? IsTickets)
        {
            if (IsTickets == null)
                IsTickets = false;

            if (SortBy == null)
                SortBy = "Newest";

            if (FilterBy == null)
                FilterBy = "";
            ViewBag.SortBy = SortBy;
            ViewBag.FilterBy = FilterBy;
            ViewBag.page = page;
            ViewBag.IsTickets = IsTickets;

            string Userid = User.Identity.GetUserId();
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            if (MembershipService.IsAuthorizedAsAdmin(Userid) || MembershipService.IsAuthorizedAsProjectManager(Userid))
            {
                ViewBag.Tickets = false;
                if ((bool)IsTickets)
                {
                    return View(ProjectService.GetAllTickets(SortBy, FilterBy).ToPagedList(pageNumber, pageSize));
                }
                return View(ProjectService.GetAllProjectsViewModel(Userid, FilterBy).ToPagedList(pageNumber, pageSize));
            }
            if (MembershipService.IsAuthorizedAsDeveloper(Userid))
            {
                ViewBag.Tickets = true;
                return View(ProjectService.GetAssignedTicketsForDevelopers(Userid, SortBy, FilterBy).ToPagedList(pageNumber, pageSize));
            }
            if (MembershipService.IsAuthorizedAsSubmitter(Userid))
            {
                ViewBag.Tickets = true;
                return View(ProjectService.GetAssignedTicketsForSubmitters(Userid, SortBy, FilterBy).ToPagedList(pageNumber, pageSize));
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddUserToRole()
        {
            string UserId = User.Identity.GetUserId();
            if (!MembershipService.IsAuthorizedAsAdmin(UserId))
            {
                return new HttpUnauthorizedResult("Sorry, you are not authorized to assign roles to users.");
            }
            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");
            ViewBag.Role = new SelectList(MembershipService.GetRoles(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserToRole(string UserId, string Role)
        {
            bool result = MembershipService.AddUserToRole(UserId, Role);
            if (result)
            {
                ViewBag.successMessage = "Great! you added a new role to the user.";
                return RedirectToAction("Dashboard");
            }
            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");
            ViewBag.Role = new SelectList(MembershipService.GetRoles(), "Name", "Name");
            return View();
        }

        [HttpGet]
        public ActionResult RemoveUserFromARole()
        {
            string UserId = User.Identity.GetUserId();
            if (!MembershipService.IsAuthorizedAsAdmin(UserId))
            {
                return new HttpUnauthorizedResult("Sorry, you are not authorized to assign roles to users.");
            }

            ViewBag.Role = new SelectList(MembershipService.GetRoles(), "Name", "Name");
            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveUserFromARole(string UserId, string Role)
        {
            bool result = MembershipService.RemoveUserFromRole(UserId, Role);
            if (result)
            {
                ViewBag.successMessage = "You removed a user from a role.";
                return RedirectToAction("Dashboard");
            }
            ViewBag.Role = new SelectList(MembershipService.GetRoles(), "Name", "Name");
            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");
            return View();
        }
    }
}