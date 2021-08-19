using BugTracker.DAL;
using BugTracker.BL;
using BugTracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Tests.BL_Tests
{
    [TestClass]
    public class ProjectServiceTests
    {
        private Mock<ApplicationDbContext> mockContext;
        private ProjectRepository projectRepo;
        private ProjectService service;
        private Mock<DbSet<Project>> mockProjectSet;
        private Mock<DbSet<ApplicationUser>> mockUserSet;
        private Mock<DbSet<ProjectUser>> mockProjectUserSet;
        private Mock<DbSet<Ticket>> mockTicketSet;
        private List<Project> Projects;
        private List<ApplicationUser> ApplicationUsers;
        private List<ProjectUser> ProjectUsers;
        private List<Ticket> Tickets;

        [TestInitialize]
        public void Setup()
        {
            Projects = new List<Project>()
            {
                new Project("QA Website") { Id = 1 },
                new Project("Task Mangement System") { Id = 2 },
                new Project("Project To Delete") { Id = 3 },
            };

            ApplicationUsers = new List<ApplicationUser>()
            {
                new ApplicationUser(){ Email = "User1@test.com", Id = "UserId1"},
                new ApplicationUser(){ Email = "User2@test.com", Id = "UserId2"},
                new ApplicationUser(){ Email = "User3@test.com", Id = "UserId3"},
                new ApplicationUser(){ Email = "User4@test.com", Id = "UserId4"},
                new ApplicationUser(){ Email = "User5@test.com", Id = "UserId5"},
                new ApplicationUser(){ Email = "User6@test.com", Id = "UserId6"},
            };

            ProjectUsers = new List<ProjectUser>()
            {
                new ProjectUser() { Id=1, ProjectId=Projects[0].Id, UserId=ApplicationUsers[0].Id },
                new ProjectUser() { Id=1, ProjectId=Projects[0].Id, UserId=ApplicationUsers[1].Id },
                new ProjectUser() { Id=1, ProjectId=Projects[0].Id, UserId=ApplicationUsers[2].Id },
                new ProjectUser() { Id=1, ProjectId=Projects[1].Id, UserId=ApplicationUsers[3].Id },
                new ProjectUser() { Id=1, ProjectId=Projects[1].Id, UserId=ApplicationUsers[4].Id },
                new ProjectUser() { Id=1, ProjectId=Projects[1].Id, UserId=ApplicationUsers[5].Id },
            };

            Tickets = new List<Ticket>()
            {
                new Ticket("Update Question", "Updating Question Class") { Id = 1 },
                new Ticket("Failing to Update DB", "Db is not updating") { Id = 2 },
                new Ticket("Create New Feature", "Create a new feature") { Id = 3 },
                new Ticket("Late on the schedule", "Need more time to finish this") { Id = 4 },
            };

            var projectsQueryable = Projects.AsQueryable();
            mockProjectSet = new Mock<DbSet<Project>>();
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projectsQueryable.Provider);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectsQueryable.Expression);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectsQueryable.ElementType);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectsQueryable.GetEnumerator());

            var ApplicationUsersQueryable = ApplicationUsers.AsQueryable();
            mockUserSet = new Mock<DbSet<ApplicationUser>>();
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(ApplicationUsersQueryable.Provider);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(ApplicationUsersQueryable.Expression);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(ApplicationUsersQueryable.ElementType);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(ApplicationUsersQueryable.GetEnumerator());

            var ProjectUsersQueryable = ProjectUsers.AsQueryable();
            mockProjectUserSet = new Mock<DbSet<ProjectUser>>();
            mockProjectUserSet.As<IQueryable<ProjectUser>>().Setup(m => m.Provider).Returns(ProjectUsersQueryable.Provider);
            mockProjectUserSet.As<IQueryable<ProjectUser>>().Setup(m => m.Expression).Returns(ProjectUsersQueryable.Expression);
            mockProjectUserSet.As<IQueryable<ProjectUser>>().Setup(m => m.ElementType).Returns(ProjectUsersQueryable.ElementType);
            mockProjectUserSet.As<IQueryable<ProjectUser>>().Setup(m => m.GetEnumerator()).Returns(ProjectUsersQueryable.GetEnumerator());

            var TicketsQueryable = Tickets.AsQueryable();
            mockTicketSet = new Mock<DbSet<Ticket>>();
            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(TicketsQueryable.Provider);
            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(TicketsQueryable.Expression);
            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(TicketsQueryable.ElementType);
            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(TicketsQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.Projects).Returns(mockProjectSet.Object);
            mockContext.Setup(c => c.ProjectUsers).Returns(mockProjectUserSet.Object);
            mockContext.Setup(c => c.Tickets).Returns(mockTicketSet.Object);
            projectRepo = new ProjectRepository(mockContext.Object);

            service = new ProjectService(mockContext.Object);
        }

        [TestMethod]
        public void ProjectServiceCreateNewProject_PassCorrectValue_NorReturn()
        {
            service.CreateNewProject("New Project");

            mockProjectSet.Verify(m => m.Add(It.IsAny<Project>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}
