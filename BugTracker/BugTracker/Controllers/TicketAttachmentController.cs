using BugTracker.BL;
using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketAttachmentController : Controller
    {
        private TicketAttachmentService ticketAttachmentService;
        private TicketService ticketService;
        public TicketAttachmentController()
        {
            var context = new ApplicationDbContext();
            ticketService = new TicketService(context);
            ticketAttachmentService = new TicketAttachmentService(context);
        }

        [HttpGet]
        public ActionResult Index(int? id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var attachments = ticketAttachmentService.GetTicketAttachments(id);
            ViewBag.TicketId = id;
            return View(attachments);
        }
        [HttpGet]
        public ActionResult Create(int? id)
        {
            ViewBag.TicketId = id;
            return View();
        }
        [HttpPost]
        public ActionResult Create(TicketAttachment ticketAttachment, HttpPostedFileBase file, int? id)
        {
            //id = 15;
            if (ticketAttachment == null || id == null)
                return null;

            var ticket = ticketService.GetTicket((int)id);
            var userId = User.Identity.GetUserId();

            if (file != null && file.ContentLength < 2100000)
            {
                string fileName = Path.GetFileName(file.FileName);
                string fileUrl = "www.bugtracker.com/" + fileName;

                file.SaveAs(Server.MapPath("/") + "/Content/" + fileName);

                ticketAttachment.FilePath = fileName;
                ticketAttachment.FileUrl = fileUrl;
                ticketAttachment.Created = DateTime.Now;
                ticketAttachment.UserId = userId;
                ticketAttachment.TicketId = ticket.Id;

                ticketAttachmentService.CreateTicketAttachment(ticketAttachment, userId, ticket.Id);
                return RedirectToAction("Index", new { @id = id });
            }

            ViewBag.TicketId = id;
            return View();
        }
        [HttpGet]
        public ActionResult DownloadFile(int? id)
        {
            if (id == null)
                return null;

            string fileName = ticketAttachmentService.GetTicketAttachment((int)id).FilePath;

            string path = Server.MapPath("~/Content/");
            string fullPath = Path.Combine(path, fileName);

            return File(fullPath, "image/jpg", fileName);
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = ticketAttachmentService.GetTicketAttachment((int)id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            
            return View(ticketAttachment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FilePath, Description, TicketId, UserId, Created, FileUrl")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ticketAttachmentService.EditTicketAttachment(ticketAttachment, userId);

                return RedirectToAction("Index");
            }

            return View(ticketAttachment);
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = ticketAttachmentService.GetTicketAttachment((int)id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = ticketAttachmentService.GetTicketAttachment((int)id);
            var userId = User.Identity.GetUserId();
            ticketAttachmentService.DeleteTicketAttachment(ticketAttachment, userId);
            return RedirectToAction("Index", new { @id = ticketAttachment.TicketId });
        }

    }
}


