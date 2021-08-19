using BugTracker.DAL;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BugTracker.BL
{
    public class TicketAttachmentService
    {
        private readonly UserRepository userRepo;
        private readonly TicketRepository ticketRepo;
        private readonly ProjectUserRepository projectUserRepo;
        private readonly TicketAttachmentRepository ticketAttachmentRepo;

        public TicketAttachmentService(ApplicationDbContext context)
        {
            this.userRepo = new UserRepository(context);
            this.projectUserRepo = new ProjectUserRepository(context);
            this.ticketRepo = new TicketRepository(context);
            this.ticketAttachmentRepo = new TicketAttachmentRepository(context);
        }
        public IEnumerable<TicketAttachment> GetTicketAttachments(int? ticketId)
        {
            if (ticketId == null)
                return null;

            var attachments = ticketAttachmentRepo.GetCollection(ta => ta.TicketId == ticketId);
            foreach (var item in attachments)
            {
                item.User = userRepo.GetEntity(item.UserId);
            }
            return attachments;
        }
        public void CreateTicketAttachment(TicketAttachment ticketAttachment, string userId, int? ticketId)
        {
            if (ticketId == null)
                return;

            var ticket = ticketRepo.GetEntity((int)ticketId);

            if (userRepo.IsUserInRole(userId, "Admin"))
            {
                ticketAttachmentRepo.Add(ticketAttachment);
            }
            else if (userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();

                if (ticket.ProjectId == projectId)
                {
                    ticketAttachmentRepo.Add(ticketAttachment);
                }
            }
            else if (userRepo.IsUserInRole(userId, "Developer") && ticket.AssignedToUserId == userId)
            {
                ticketAttachmentRepo.Add(ticketAttachment);
            }
            else if (userRepo.IsUserInRole(userId, "Submitter") && ticket.OwnerUserId == userId)
            {
                ticketAttachmentRepo.Add(ticketAttachment);
            }
        }
        public void EditTicketAttachment(TicketAttachment ticketAttachment, string userId)
        {
            if (userId == null)
                return;

            if (userRepo.IsUserInRole(userId, "Admin"))
            {
                ticketAttachmentRepo.Update(ticketAttachment);
            }
            else if (userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
                var ticket = ticketRepo.GetEntity(t => t.ProjectId == projectId);

                if (ticketAttachment.TicketId == ticket.Id)
                {
                    ticketAttachmentRepo.Update(ticketAttachment);
                }
            }
            else if (userRepo.IsUserInRole(userId, "Developer") || userRepo.IsUserInRole(userId, "Submitter") && ticketAttachment.UserId == userId)
            {
                ticketAttachmentRepo.Update(ticketAttachment);
            }
        }
        public void DeleteTicketAttachment(TicketAttachment ticketAttachment, string userId)
        {
            if (userId == null)
                return;

            if (userRepo.IsUserInRole(userId, "Admin"))
            {
                ticketAttachmentRepo.Delete(ticketAttachment);
            }
            else if (userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
                var ticket = ticketRepo.GetEntity(t => t.ProjectId == projectId);

                if (ticketAttachment.TicketId == ticket.Id)
                {
                    ticketAttachmentRepo.Delete(ticketAttachment);
                }
            }
            else if (userRepo.IsUserInRole(userId, "Developer") || userRepo.IsUserInRole(userId, "Submitter") && ticketAttachment.UserId == userId)
            {
                ticketAttachmentRepo.Delete(ticketAttachment);
            }
        }
        public TicketAttachment GetTicketAttachment(int? id)
        {
            if (id == null)
                return null;

            return ticketAttachmentRepo.GetEntity((int)id);
        }
    }
}