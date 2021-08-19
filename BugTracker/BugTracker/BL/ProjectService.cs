using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.BL
{
    [Authorize(Roles = "Admin, Project Manager")]
    public class ProjectService
    {
        private readonly UserRepository userRepo;
        private readonly ProjectRepository projectRepo;
        private readonly ProjectUserRepository projectUserRepo;
        private readonly TicketService TicketService;

        public ProjectService(ApplicationDbContext context)
        {
            this.userRepo = new UserRepository(context);
            this.projectRepo = new ProjectRepository(context);
            this.projectUserRepo = new ProjectUserRepository(context);
            this.TicketService = new TicketService(context);
        }

        public ICollection<Ticket> GetAllTickets(string SortBy, string FilterBy)
        {
            var Tickets = TicketService.SearchForTickets(FilterBy);
            return SortTickets(SortBy, Tickets).ToList();
        }


        public void CreateNewProject(string name)
        {
            if (name == null)
                return;
            var project = new Project(name);
            AddProject(project);
        }

        public void AddProject(Project project)
        {
            projectRepo.Add(project);
        }
        public void DeleteProject(int projectId)
        {
            var project = projectRepo.GetEntity(projectId);
            if (project == null)
                return;

            projectRepo.Delete(project);
        }
        public void UpdateProject(int projectId)
        {
            var project = projectRepo.GetEntity(projectId);
            if (project == null)
                return;

            projectRepo.Update(project);
        }
        public bool AssignUserToProject(int projectId, string userId)
        {
            var project = projectRepo.GetEntity(projectId);
            if (project == null)
                return false;

            var user = userRepo.GetEntity(userId);
            if (user == null)
                return false;

            ProjectUser projectUser = new ProjectUser(project.Id, user.Id);

            projectUserRepo.Add(projectUser);
            return true;
        }
        public bool RemoveUserFromProject(int projectId, string userId)
        {
            var project = projectRepo.GetEntity(projectId);
            if (project == null)
                return false;

            var user = userRepo.GetEntity(userId);
            if (user == null)
                return false;

            ProjectUser projectUser = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == user.Id);

            projectUserRepo.Delete(projectUser);
            return true;
        }

        public IEnumerable<ProjectManagerViewModel> SearchforProjects(string keyword, IEnumerable<ProjectManagerViewModel> projects)
        {
            if (keyword != null)
            {
                return projects.Where(p => p.ProjectName.ToLower().Contains(keyword.ToLower())
                    || p.ProjectManagerName.ToLower().Contains(keyword.ToLower()));
            }
            return null;
        }
        public IEnumerable<Project> GetAllProjects(string userId)
        {
            if (userRepo.IsUserInRole(userId, "Admin") || userRepo.IsUserInRole(userId, "Project Manager"))
            {
                return projectRepo.GetCollection();
            }
            else if (userRepo.IsUserInRole(userId, "Developer"))
            {
                return projectRepo.GetCollection(p => p.Tickets.Any(t => t.AssignedToUserId == userId));
            }
            else if (userRepo.IsUserInRole(userId, "Submitter"))
            {
                return projectRepo.GetCollection(p => p.Tickets.Any(t => t.OwnerUserId == userId));
            }
            return null;
        }

        public IEnumerable<ProjectManagerViewModel> GetAllProjectsViewModel(string userId, string FilterBy)
        {
            if (userRepo.IsUserInRole(userId, "Project Manager"))
            {
                //Make a list of ProjectmanagerViewModel by taking data from projects and their linked projectUsers and return that list

                var projectsList = projectRepo.GetCollection()
                        .Select(p => new ProjectManagerViewModel
                        {
                            ProjectId = p.Id,
                            ProjectName = p.Name,
                            ProjectManagerName = p.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId && userRepo.IsUserInRole(pu.UserId, "Project Manager")) == null ? "N/A" : p.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId && userRepo.IsUserInRole(pu.UserId, "Project Manager")).user.UserName,
                        }).OrderByDescending(p => p.ProjectId);
                if (projectsList.Count() == 0)
                    return new List<ProjectManagerViewModel>();
                return SearchforProjects(FilterBy, projectsList);
            }
            else if (userRepo.IsUserInRole(userId, "Admin"))
            {
                var projectsList = projectRepo.GetCollection()
                        .Select(p => new ProjectManagerViewModel
                        {
                            ProjectId = p.Id,
                            ProjectName = p.Name,
                            ProjectManagerName = p.ProjectUsers.FirstOrDefault(pu => userRepo.IsUserInRole(pu.UserId, "Project Manager")) == null ? "N/A" : p.ProjectUsers.FirstOrDefault(pu => userRepo.IsUserInRole(pu.UserId, "Project Manager")).user.UserName,
                        }).OrderByDescending(p => p.ProjectId);
                if (projectsList.Count() == 0)
                    return new List<ProjectManagerViewModel>();
                return SearchforProjects(FilterBy, projectsList);
            }
            return null;
        }

        [Authorize(Roles = "Developer, Submitter")]
        public IEnumerable<Project> GetDeveloperAndSubmitterProjects(string userId)
        {
            if (userRepo.IsUserInRole(userId, "Developer") || userRepo.IsUserInRole(userId, "Submitter"))
            {
                return projectUserRepo.GetCollection(pu => pu.UserId == userId).Select(p => p.Project);
            }
            return null;
        }


        [Authorize(Roles = "Developer, Submitter")]
        public IEnumerable<ProjectManagerViewModel> GetAssignedProjectsOfDeveloperAndSubmitter(string userId)
        {
            if (userRepo.IsUserInRole(userId, "Developer"))
            {
                return projectRepo.GetCollection(p => p.Tickets
                                                       .Any(t => t.AssignedToUserId == userId))
                                                       .Select(p => new ProjectManagerViewModel
                                                       {
                                                           ProjectId = p.Id,
                                                           ProjectName = p.Name,
                                                           ProjectManagerName = p.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId && userRepo.IsUserInRole(pu.UserId, "Project Manager")) == null ? "N/A" : p.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId && userRepo.IsUserInRole(pu.UserId, "Project Manager")).user.UserName,
                                                       });

            }
            else if (userRepo.IsUserInRole(userId, "Submitter"))
            {
                return projectRepo.GetCollection(p => p.Tickets
                                                       .Any(t => t.OwnerUserId == userId))
                                                       .Select(p => new ProjectManagerViewModel
                                                       {
                                                           ProjectId = p.Id,
                                                           ProjectName = p.Name,
                                                           ProjectManagerName = p.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId && userRepo.IsUserInRole(pu.UserId, "Project Manager")) == null ? "N/A" : p.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId && userRepo.IsUserInRole(pu.UserId, "Project Manager")).user.UserName,
                                                       });
            }
            return null;
        }

        public IEnumerable<Ticket> GetAssignedTicketsForDevelopers(string UserId, string SortBy, string FilterBy)
        {
            var Tickets = TicketService.SearchForTickets(FilterBy).Where(t => t.AssignedToUserId == UserId);
            return SortTickets(SortBy, Tickets);
        }

        public IEnumerable<Ticket> GetAssignedTicketsForSubmitters(string UserId, string SortBy, string FilterBy)
        {
            if (FilterBy == null)
                FilterBy = "";
            var Tickets = TicketService.SearchForTickets(FilterBy).Where(t => t.OwnerUserId == UserId);
            return SortTickets(SortBy, Tickets);
        }

        public IEnumerable<Ticket> SortTickets(string SortBy, IEnumerable<Ticket> tickets)
        {
            switch (SortBy)
            {
                case "Title":
                    tickets.OrderByDescending(t => t.Title);
                    break;
                case "Owner":
                    tickets.OrderByDescending(t => t.OwnerUser.UserName);
                    break;
                case "Assignment":
                    tickets.OrderByDescending(t => t.AssignedToUser.UserName);
                    break;
                case "UpdateDate":
                    tickets.OrderByDescending(t => t.Updated);
                    break;
                case "Type":
                    tickets.OrderByDescending(t => t.TicketType.Name);
                    break;
                case "Priority":
                    tickets.OrderByDescending(t => t.TicketPriorityId);
                    break;
                case "Status":
                    tickets.OrderByDescending(t => t.TicketStatusId);
                    break;
                case "project":
                    tickets.OrderByDescending(t => t.Project.Id);
                    break;
                default:
                case "CreatedDate":
                    tickets.OrderByDescending(t => t.Created);
                    break;
            }

            return tickets;
        }
    }
}