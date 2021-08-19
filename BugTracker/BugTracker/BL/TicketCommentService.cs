using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.BL
{
    public class TicketCommentService
    {
        private readonly UserRepository userRepo;
        private readonly TicketRepository ticketRepo;
        private readonly ProjectUserRepository projectUserRepo;
        private readonly TicketCommentRepository ticketCommentRepo;

        public TicketCommentService(ApplicationDbContext context)
        {
            this.userRepo = new UserRepository(context);
            this.projectUserRepo = new ProjectUserRepository(context);
            this.ticketRepo = new TicketRepository(context);
            this.ticketCommentRepo = new TicketCommentRepository(context);
        }
        public IEnumerable<TicketComment> GetTicketComments(int? ticketId)
        {
            if (ticketId == null)
                return null;

            return ticketCommentRepo.GetCollection(tc => tc.TicketId == ticketId);
        }
        public void CreateTicketComment(string userId, int ticketId, string comment)
        {
            if (userId == null)
                return;

            var ticket = ticketRepo.GetEntity(t => t.Id == ticketId);
            TicketComment ticketComment = new TicketComment
            {
                Comment = comment,
                Created = DateTime.Now,
                TicketId = ticketId,
                UserId = userId
            };
            if(userRepo.IsUserInRole(userId, "Admin"))
            {
                ticketCommentRepo.Add(ticketComment);
                var text = $"A new comment has been posted on your ticket by {ticketComment.User.UserName}. '{ticketComment.Comment}'";
                var subject = $"Bug Tracker - {ticket.Title} has a new Comment by {ticketComment.User.UserName}";
                EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);

            }
            else if(userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
  
                if(ticket.ProjectId == projectId)
                {
                    ticketCommentRepo.Add(ticketComment);
                    var text = $"A new comment has been posted on your ticket by {ticketComment.User.UserName}. '{ticketComment.Comment}'";
                    var subject = $"Bug Tracker - {ticket.Title} has a new Comment by {ticketComment.User.UserName}";
                    EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);
                }
            }
            else if(userRepo.IsUserInRole(userId, "Developer") && ticket.AssignedToUserId == userId)
            {
                ticketCommentRepo.Add(ticketComment);
                var text = $"A new comment has been posted on your ticket by {ticketComment.User.UserName}. '{ticketComment.Comment}'";
                var subject = $"Bug Tracker - {ticket.Title} has a new Comment by {ticketComment.User.UserName}";
                EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);
            }
            else if(userRepo.IsUserInRole(userId, "Submitter") && ticket.OwnerUserId == userId)
            {
                ticketCommentRepo.Add(ticketComment);
                var text = $"A new comment has been posted on your ticket by {ticketComment.User.UserName}. '{ticketComment.Comment}'";
                var subject = $"Bug Tracker - {ticket.Title} has a new Comment by {ticketComment.User.UserName}";
                EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);
            }
        }
        public void EditTicketComment(TicketComment ticketComment, string userId)
        {
            if (userId == null)
                return;

            if(userRepo.IsUserInRole(userId, "Admin"))
            {
                ticketCommentRepo.Update(ticketComment);
            }
            else if(userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
                var ticket = ticketRepo.GetEntity(t => t.ProjectId == projectId);

                if(ticketComment.TicketId == ticket.Id)
                {
                    ticketCommentRepo.Update(ticketComment);
                }
            }
            else if(userRepo.IsUserInRole(userId, "Developer") || userRepo.IsUserInRole(userId, "Submitter")  && ticketComment.UserId == userId)
            {
                ticketCommentRepo.Update(ticketComment);
            }
        }
        public void DeleteTicketComment(TicketComment ticketComment, string userId)
        {
            if (userId == null)
                return;

            if (userRepo.IsUserInRole(userId, "Admin"))
            {
                ticketCommentRepo.Delete(ticketComment);
            }
            else if (userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
                var ticket = ticketRepo.GetEntity(t => t.ProjectId == projectId);

                if (ticketComment.TicketId == ticket.Id)
                {
                    ticketCommentRepo.Delete(ticketComment);
                }
            }
            else if (userRepo.IsUserInRole(userId, "Developer") || userRepo.IsUserInRole(userId, "Submitter")  && ticketComment.UserId == userId)
            {
                ticketCommentRepo.Delete(ticketComment);
            }
        }
    }
}