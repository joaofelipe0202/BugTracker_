using BugTracker.DAL;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Threading.Tasks;

namespace BugTracker.BL
{
    public class TicketNotificationService
    {
        private readonly UserRepository userRepo;
        private readonly TicketRepository ticketRepo;
        private readonly TicketNotificationRepository ticketNotificationRepo;
        public TicketNotificationService(ApplicationDbContext context)
        {
            this.userRepo = new UserRepository(context);
            this.ticketRepo = new TicketRepository(context);
            this.ticketNotificationRepo = new TicketNotificationRepository(context);
        }

        public void GenerateNotificationWhenAssigned(string userId, int? ticketId)
        {
            if (userId == null || ticketId == null)
                return;

            var ticket = ticketRepo.GetEntity((int)ticketId);
            var user = userRepo.GetEntity(userId);

            if (ticket.AssignedToUserId == user.Id && user.TicketNotifications.FirstOrDefault(tn => tn.TicketId == ticket.Id) == null)
            {
                var ticketNotification = new TicketNotification { TicketId = ticket.Id, UserId = user.Id };
                
                ticketNotificationRepo.Add(ticketNotification);

                var text = $"You have been assinged to the {ticket.Title}. It is currently priority {ticket.TicketPriority.Name}";
                var subject = "Bug Tracker - Asssinged to Ticket " + ticket.Title;
                EmailManager.SendEmail(user.UserName, user.Email, text, subject);
            }
        }
        public void GenerateNotificationWhenEdited(string userId, string ticketStatus, int? ticketId)
        {
            if (userId == null || ticketStatus == null || ticketId == null)
                return;

            var ticket = ticketRepo.GetEntity((int)ticketId);

            if(ticket.AssignedToUserId != userId && ticketStatus == "Modified")
            {
                TicketNotification ticketNotification = new TicketNotification { TicketId = ticket.Id, UserId = ticket.AssignedToUserId };

                ticketNotificationRepo.Add(ticketNotification);
            }
        }     
    }
}