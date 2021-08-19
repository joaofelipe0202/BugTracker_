using BugTracker.BL;
using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class TicketHistoryController : Controller
    {
        private TicketService TicketService;
        private TicketHistoryService TicketHistoryService;

        public TicketHistoryController()
        {
            var context = new ApplicationDbContext();
            this.TicketService = new TicketService(context);
            this.TicketHistoryService = new TicketHistoryService(context);
        }

        public ActionResult Index(int? id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string UserId = User.Identity.GetUserId();
            ViewBag.TicketId = id;

            var history = TicketHistoryService.GetTicketHistory(id, UserId);
            return View(history);
        }
    }
}