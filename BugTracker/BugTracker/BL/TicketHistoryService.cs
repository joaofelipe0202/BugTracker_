using BugTracker.DAL;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.BL
{
    public class TicketHistoryService
    {
        private readonly UserRepository userRepo;
        private readonly TicketRepository ticketRepo;
        private readonly ProjectUserRepository projectUserRepo;
        private readonly TicketHistoryRepository ticketHistoryRepo;

        public TicketHistoryService(ApplicationDbContext context)
        {
            this.userRepo = new UserRepository(context);
            this.projectUserRepo = new ProjectUserRepository(context);
            this.ticketRepo = new TicketRepository(context);
            this.ticketHistoryRepo = new TicketHistoryRepository(context);
        }
        public IEnumerable<TicketHistory> GetTicketHistory(int? ticketId, string userId)
        {
            if (ticketId == null || userId == null)
                return null;

            var ticket = ticketRepo.GetEntity((int)ticketId);
            if(userRepo.IsUserInRole(userId, "Admin"))
            {
                return ticketHistoryRepo.GetCollection(th => th.TicketId == ticket.Id).ToList();
            }
            else if(userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
                if(ticket.ProjectId == projectId)
                {
                    return ticketHistoryRepo.GetCollection(th => th.TicketId == ticket.Id).ToList();
                }
            }
            else if(userRepo.IsUserInRole(userId, "Developer") && ticket.AssignedToUserId == userId)
            {
                return ticketHistoryRepo.GetCollection(th => th.TicketId == ticket.Id).ToList();
            }
            else if(userRepo.IsUserInRole(userId, "Submitter") && ticket.OwnerUserId == userId)
            {
                return ticketHistoryRepo.GetCollection(th => th.TicketId == ticket.Id).ToList();
            }
            return null;
        }
    }
}