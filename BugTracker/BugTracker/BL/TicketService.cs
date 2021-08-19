
using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;


namespace BugTracker.BL
{
    public class TicketService
    {
        public  ApplicationDbContext db;
        private readonly MembershipService MembershipService;
        private readonly TicketNotificationService TicketNotificationService;

        private readonly UserRepository userRepo;
        private readonly TicketRepository ticketRepo;
        private readonly ProjectUserRepository projectUserRepo;
        private readonly TicketStatusRepository ticketStatusRepo;
        private readonly TicketTypeRepository ticketTypeRepo;
        private readonly TicketPriorityRepository ticketPriorityRepo;
        private readonly TicketHistoryRepository ticketHistoryRepo;

        public TicketService(ApplicationDbContext context)
        {
            this.db = context;
            this.MembershipService = new MembershipService(context);
            this.TicketNotificationService = new TicketNotificationService(context);
            this.userRepo = new UserRepository(context);
            this.projectUserRepo = new ProjectUserRepository(context);
            this.ticketRepo = new TicketRepository(context);
            this.ticketStatusRepo = new TicketStatusRepository(context);
            this.ticketTypeRepo = new TicketTypeRepository(context);
            this.ticketPriorityRepo = new TicketPriorityRepository(context);
            this.ticketHistoryRepo = new TicketHistoryRepository(context);
        }

        public Ticket GetTicket(int id)
        {
            return ticketRepo.GetEntity(id);
        }

       

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public TicketsViewModel FillTicketsDetails(int id)
        {
            Ticket ticket = ticketRepo.GetEntity(id);
            TicketsViewModel tvm;
            if (ticket.AssignedToUser == null)
            {
                tvm = new TicketsViewModel(ticket.Id, ticket.Title, ticket.Description /*ticket.OwnerUser.UserName*/,"name1", "Not Assigned", ticket.Created, ticket.Updated, ticket.Project.Name, ticket.TicketType, ticket.TicketPriority, ticket.TicketStatus);
            } else
            {
                tvm = new TicketsViewModel(ticket.Id, ticket.Title, ticket.Description/* ticket.OwnerUser.UserName,*/, "name1", ticket.AssignedToUser.UserName, ticket.Created, ticket.Updated, ticket.Project.Name, ticket.TicketType, ticket.TicketPriority, ticket.TicketStatus);
            }
            
            return tvm;
        }

        [Authorize(Roles = "Submitter")]
        public void AddTicket(Ticket ticket)
        {
            ticketRepo.Add(ticket);
        }

        [Authorize(Roles = "Submitter")]
        public void CreateTicket(string Title, string Description, int projectId, int TicketTypeId, int TicketPriorityId, int TicketStatusId, string OwnerUserId)
        {
            var projectUserId = projectUserRepo.GetCollection(pu => pu.ProjectId == projectId && pu.UserId == OwnerUserId)
                .Select(p => p.UserId).FirstOrDefault();

            if (projectUserId == null)
                return;

            Ticket ticket = new Ticket 
            {
                Title = Title,
                Description = Description,
                ProjectId = projectId,
                TicketTypeId = TicketTypeId,
                TicketPriorityId = TicketPriorityId,
                TicketStatusId = TicketStatusId,
                OwnerUserId = projectUserId
            };
            ticketRepo.Add(ticket);
        }

        public void EditTicketHighLevel(string userId, int ticketId, string property, string newValue)
        {
            if (userId == null)
                return;

            var ticket = ticketRepo.GetEntity(ticketId);
            var ticketProperty = ticket.GetType().GetProperties().FirstOrDefault(tp => tp.Name == property);

            var ticketPropertyValue = ticketProperty.GetValue(ticket, null).ToString();

            if (userRepo.IsUserInRole(userId, "Admin"))
            {
                if (ticketPropertyValue != newValue)
                {
                    TicketHistory ticketHistory = new TicketHistory
                    {
                        Property = ticketProperty.Name,
                        OldValue = ticketPropertyValue,
                        NewValue = newValue,
                        TicketId = ticket.Id,
                        UserId = userId
                    };
                    ticketHistoryRepo.Add(ticketHistory);
                    ticketPropertyValue = newValue;
                    ticketProperty.SetValue(ticket, newValue);

                    ticket.Updated = DateTime.Now;

                    var text = $"Your tickets {property} has been changed to {newValue}";
                    var subject = $"Bug Tracker - {ticket.Title} {property} has been changed ";
                    EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);

                    ticketRepo.Update(ticket);
                }
            }
            else if (userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
                if (ticketPropertyValue != newValue && ticket.ProjectId == projectId && ticketPropertyValue != newValue)
                {
                    TicketHistory ticketHistory = new TicketHistory
                    {
                        Property = ticketProperty.Name,
                        OldValue = ticketPropertyValue,
                        NewValue = newValue,
                        TicketId = ticket.Id,
                        UserId = userId
                    };
                    ticketHistoryRepo.Add(ticketHistory);
                    ticketPropertyValue = newValue;
                    ticketProperty.SetValue(ticket, newValue);


                    var text = $"Your tickets {property} has been changed to {newValue}";
                    var subject = $"Bug Tracker - {ticket.Title} {property} has been changed ";
                    EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);

                    ticketRepo.Update(ticket);
                }
            }
            if (userRepo.IsUserInRole(userId, "Developer") && ticket.AssignedToUserId == userId && ticketPropertyValue != newValue && ticketProperty.ToString() != ticket.TicketStatus.ToString())
            {
                TicketHistory ticketHistory = new TicketHistory
                {
                    Property = ticketProperty.Name,
                    OldValue = ticketPropertyValue,
                    NewValue = newValue,
                    TicketId = ticket.Id,
                    UserId = userId
                };
                ticketHistoryRepo.Add(ticketHistory);
                ticketPropertyValue = newValue;
                ticketProperty.SetValue(ticket, newValue);

                var text = $"Your tickets {property} has been changed to {newValue}";
                var subject = $"Bug Tracker - {ticket.Title} {property} has been changed ";
                EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);

                ticketRepo.Update(ticket);
            }
            else if (userRepo.IsUserInRole(userId, "Submitter") && ticket.OwnerUserId == userId && ticketPropertyValue != newValue && ticketProperty.ToString() != ticket.TicketStatus.ToString())
            {
                TicketHistory ticketHistory = new TicketHistory
                {
                    Property = ticketProperty.Name,
                    OldValue = ticketPropertyValue,
                    NewValue = newValue,
                    TicketId = ticket.Id,
                    UserId = userId
                };
                ticketHistoryRepo.Add(ticketHistory);
                ticketPropertyValue = newValue;

                ticketProperty.SetValue(ticket, newValue);

                var text = $"Your tickets {property} has been changed to {newValue}";
                var subject = $"Bug Tracker - {ticket.Title} {property} has been changed ";
                EmailManager.SendEmail(ticket.AssignedToUser.UserName, ticket.AssignedToUser.Email, subject, text);

                ticketRepo.Update(ticket);
            }
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        public bool AssignUserToTicket( string userId, int? ticketId)
        {
            if (ticketId == null || userId == null)
                return false;

            var ticket = ticketRepo.GetEntity(t => t.Id == ticketId);
            var user = userRepo.GetEntity(userId);

            if (userRepo.IsUserInRole(user.Id, "Developer"))
            {
                ticket.AssignedToUserId = user.Id;
                ticketRepo.SaveChanges();
                return true;
            }
            ticketRepo.SaveChanges();
            this.TicketNotificationService.GenerateNotificationWhenAssigned(userId, ticketId);
            
            return false;
        }

        public bool RemoveUserFromTicket(int ticketId, string userId)
        {
            var ticket = ticketRepo.GetEntity(ticketId);
            if (ticket == null)
                return false;

            var user = userRepo.GetEntity(userId);
            if (user == null)
                return false;

            ticket.AssignedToUserId = null;
            return true;
        }

        public IEnumerable<Ticket> GetTickets(int? id)
        {
            if (id == null)
                return null;

            return ticketRepo.GetCollection(t => t.ProjectId == id);
        }

        public IEnumerable<Ticket> GetTickets()
        {
            return ticketRepo.GetCollection();
        }

        public IQueryable<Ticket> GetTickets(string userId)
        {
            if (userId == null)
                return null;

            if(userRepo.IsUserInRole(userId, "Submitter"))
            {
                return ticketRepo.GetCollection(t => t.OwnerUserId == userId);
            }
            else if(userRepo.IsUserInRole(userId, "Project Manager"))
            {
                var projectId = projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.ProjectId).FirstOrDefault();
                return ticketRepo.GetCollection(t => t.ProjectId == projectId);
            }
            else if(userRepo.IsUserInRole(userId, "Developer"))
            {
                return ticketRepo.GetCollection(t => t.AssignedToUserId == userId);
            }
            return ticketRepo.GetCollection();
        }
        public IEnumerable<Ticket> SearchForTickets(string keyword)
        {
            if (keyword != null && keyword.Equals(keyword.ToLower()))
            {
                return ticketRepo.GetCollection(t => t.Title.ToLower().Contains(keyword.ToLower()) 
                    || t.Description.ToLower().Contains(keyword.ToLower()) || t.Project.Name.ToLower().Contains(keyword) 
                        || t.TicketType.Name.ToLower().Contains(keyword.ToLower()) || t.TicketType.Name.ToLower().Contains(keyword.ToLower())
                            || t.TicketStatus.Name.ToLower().Contains(keyword.ToLower()));
            }
            
            return null;  
        }
        public IEnumerable<Ticket> SortTickets(string SortBy, IEnumerable<Ticket> tickets)
        {
            switch (SortBy)
            {
                case "Title":
                    return tickets.OrderByDescending(t => t.Title).ToList();
                case "Description":
                    return tickets.OrderByDescending(t => t.Description).ToList();
                case "OwnerName":
                    return tickets.OrderByDescending(t => t.OwnerUser.UserName).ToList();
                case "AssignedName":
                    return tickets.OrderByDescending(t => t.AssignedToUser.UserName).ToList();
                case "CreationDate":
                    return tickets.OrderByDescending(t => t.Created).ToList();
                case "UpdateDate":
                    return tickets.OrderByDescending(t => t.Updated).ToList();
                case "Type":
                    return tickets.OrderByDescending(t => t.TicketType.Name).ToList();
                case "Priority":
                    return tickets.OrderByDescending(t => t.TicketPriority.Name).ToList();
                case "Status":
                    return tickets.OrderByDescending(t => t.TicketStatus.Name).ToList();
                case "Project":
                    return tickets.OrderByDescending(t => t.Project.Name).ToList();
                default:
                    return tickets;
            }
        }
        public IEnumerable<TicketType> GetAllTicketsType()
        {
            return ticketTypeRepo.GetCollection();
        }
        public IEnumerable<TicketStatus> GetAllTicketsStatus()
        {
            return ticketStatusRepo.GetCollection();
        }
        public IEnumerable<TicketPriority> GetAllTicketsPriority()
        {
            return ticketPriorityRepo.GetCollection();
        }

        public ApplicationUser GetProjectManager(Ticket ticket)
        {
            return ticket.Project.ProjectUsers.FirstOrDefault(pu => MembershipService.IsAuthorizedAsProjectManager(pu.UserId)).user;

        }
    }
}