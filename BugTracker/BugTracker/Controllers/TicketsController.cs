using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.BL;
using BugTracker.DAL;
using Microsoft.AspNet.Identity;
using System.IO;
using PagedList;


namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ProjectService ProjectService;
        private readonly MembershipService MembershipService;
        private readonly TicketCommentService TicketCommentService;
        private readonly TicketAttachmentService TicketAttachmentService;
        private readonly TicketService TicketService;
        // GET: Tickets

        public TicketsController()
        {
            db = new ApplicationDbContext();
            this.ProjectService = new ProjectService(db);
            this.MembershipService = new MembershipService(db);
            this.TicketCommentService = new TicketCommentService(db);
            this.TicketAttachmentService = new TicketAttachmentService(db);
            this.TicketService = new TicketService(db);
        }

        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();
            var tickets = TicketService.GetTickets(UserId);
            return View(tickets.ToList());
        }

        public ActionResult GetAllTickets()
        {
            var tickets = TicketService.GetTickets();
            return View(tickets.ToList());
        }



        [Authorize(Roles ="Admin, Project Manager")]
        [HttpGet]
        public ActionResult AssignTicketToUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string UserId = User.Identity.GetUserId();
            if (!MembershipService.IsAuthorizedAsAdmin(UserId) && !MembershipService.IsAuthorizedAsProjectManager(UserId))
            {
                return new HttpUnauthorizedResult("Sorry, you are not authorized to assign Tickets to users.");
            }
            ViewBag.TicketId = id;
            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTicketToUser(int? id, string UserId)
        {
            bool result = TicketService.AssignUserToTicket(UserId, (int)id);
            if (result)
            {
                ViewBag.successMessage = "Great! you added a new role to the user.";
                return RedirectToAction("Details", new { @id = id});
            }
            
            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");
            return View();
        }

        [Authorize(Roles = "Admin, Project Manager")]
        [HttpGet]
        public ActionResult RemoveUserFromATicket(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.TicketId = id;
            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveUserFromATicket(int? id, string UserId)
        {
            if (id == null || string.IsNullOrEmpty(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            bool result = TicketService.RemoveUserFromTicket((int)id, UserId);
            if (result)
            {
                ViewBag.successMessage = "You removed a developer from a ticket.";
                return RedirectToAction("Details", new { @id = id });
            }

            ViewBag.UserId = new SelectList(MembershipService.GetUsers(), "Id", "UserName");
            return View();
        }

        public ActionResult AllTicketsOfAProject(int? id)
        {
            if (id == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View((List<Ticket>)TicketService.GetTickets(id).ToList());
        }

        public bool CanAccessTicket(Ticket ticket)
        {
            string Userid = User.Identity.GetUserId();
            if (ticket.OwnerUserId == Userid)
                return true;
            if (ticket.AssignedToUserId == Userid)
                return true;
            if (MembershipService.IsAuthorizedAsAdmin(Userid))
                return true;
            if (TicketService.GetProjectManager(ticket).Id == Userid)
                return true;
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager,Developer, Submitter")]
        public ActionResult ChangeTicketDeveloper(int? id, string AssignedToUserId)
        {
            string UserId = User.Identity.GetUserId();

            if (id == null || AssignedToUserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            bool result = CanAccessTicket(ticket);
            if (!result)
            {
                return new HttpUnauthorizedResult();
            }
            TicketService.EditTicketHighLevel(UserId, (int)id, "TicketStatusId", AssignedToUserId);

            return RedirectToAction("Details", new { @id = id });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager,Developer, Submitter")]

        public ActionResult ChangeTicketStatus(int? id, int? ticketStatusId)
        {
            string UserId = User.Identity.GetUserId();

            if (id == null || ticketStatusId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            bool result = CanAccessTicket(ticket);
            if (!result)
            {
                return new HttpUnauthorizedResult();
            }
            TicketService.EditTicketHighLevel(UserId, (int)id, "TicketStatusId",ticketStatusId.ToString());

            return RedirectToAction("Details", new { @id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager,Developer, Submitter")]
        public ActionResult ChangeTicketPriority(int? id, int? TicketPriorityId)
        {
            string UserId = User.Identity.GetUserId();
            if (id == null || TicketPriorityId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            bool result = CanAccessTicket(ticket);
            if (!result)
            {
                return new HttpUnauthorizedResult();
            }
            TicketService.EditTicketHighLevel(UserId,(int)id, "TicketPriority", TicketPriorityId.ToString());
            return RedirectToAction("Details",new { @id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer,Submitter")]
        public ActionResult ChangeTicketType(int? id, int? ticketTypeId)
        {
            string UserId = User.Identity.GetUserId();

            if (id == null || ticketTypeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            bool result = CanAccessTicket(ticket);
            if (!result)
            {
                return new HttpUnauthorizedResult();
            }
            TicketService.EditTicketHighLevel(UserId, (int)id, "TicketType", ticketTypeId.ToString());
            return RedirectToAction("Details", new { @id = id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer,Submitter")]
        public ActionResult ChangeTicketTitle(int? id, string Title)
        {
            string UserId = User.Identity.GetUserId();
            if (id == null || Title == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            bool result = CanAccessTicket(ticket);
            if (!result)
            {
                return new HttpUnauthorizedResult();
            }
            TicketService.EditTicketHighLevel(UserId, (int)id, "Title", Title);
            return RedirectToAction("Details", new { @id = id });

        }

         [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager,Developer, Submitter")]
        public ActionResult ChangeTicketDescription(int? id, string Description)
        {
            string UserId = User.Identity.GetUserId();

            if (id == null || Description == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            bool result = CanAccessTicket(ticket);
            if (!result)
            {
                return new HttpUnauthorizedResult();
            }
            TicketService.EditTicketHighLevel(UserId, (int)id, "Title", Description);
            return RedirectToAction("Details", new { @id = id });

        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            bool result = CanAccessTicket(ticket);
            if (!result)
            {
                return new HttpUnauthorizedResult();
            }
            ViewBag.TicketStatusId = new SelectList(TicketService.GetAllTicketsStatus(), "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(TicketService.GetAllTicketsType(), "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(TicketService.GetAllTicketsPriority(), "Id", "Name");
            TicketsViewModel tvm = TicketService.FillTicketsDetails((int)id);
            return View(tvm);

        }

        [Authorize(Roles ="Submitter")]
        // GET: Tickets/Create
        public ActionResult Create(int? id)
        {
            if(id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string UserId = User.Identity.GetUserId();

            ViewBag.ProjectId = id;
            ViewBag.TicketTypeId = new SelectList(TicketService.GetAllTicketsType(), "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(TicketService.GetAllTicketsPriority(), "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id,[Bind(Include = "Id,Title,Description,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            string UserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                ticket.ProjectId = (int)id;
                ticket.OwnerUserId = UserId;
                ticket.TicketStatusId = 1;
                ticket.Created = DateTime.Now;
                ticket.Updated = DateTime.Now;
                TicketService.AddTicket(ticket);
                return RedirectToAction("Dashboard", "Users");
            }

            ViewBag.ProjectId = id;
            ViewBag.TicketTypeId = new SelectList(TicketService.GetAllTicketsType(), "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(TicketService.GetAllTicketsPriority(), "Id", "Name");
            return View();
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = TicketService.GetTicket((int)id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedToUserId = new SelectList(MembershipService.GetUsers(), "Id", "Email");
            ViewBag.TicketStatusId = new SelectList(TicketService.GetAllTicketsStatus(), "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(TicketService.GetAllTicketsType(), "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(TicketService.GetAllTicketsPriority(), "Id", "Name");
            return View(ticket);
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
