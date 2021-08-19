using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class ProjectRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public ProjectRepository repo;
        public Mock<DbSet<Project>> mockSet;
        List<Project> projects;

        [TestInitialize]
        public void Setup()
        {
            projects = new List<Project>()
            {
                new Project("QA Website") { Id = 1 },
                new Project("Task Mangement System") { Id = 2 },
                new Project("Project To Delete") { Id = 3 },
            };

            var projectsQueryable = projects.AsQueryable();
            mockSet = new Mock<DbSet<Project>>();
            mockSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projectsQueryable.Provider);
            mockSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectsQueryable.Expression);
            mockSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectsQueryable.ElementType);
            mockSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectsQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Projects).Returns(mockSet.Object);
            repo = new ProjectRepository(mockContext.Object);
        }

        [TestMethod]
        public void ProjectRepositoryAdd_PassNewProject__NoReturnValueDbSaves()
        {
            repo.Add(new Project("New Test Project"));

            mockSet.Verify(m => m.Add(It.IsAny<Project>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void ProjectRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<Project>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void ProjectRepositoryDelete_PassProject__NoReturnValueDbSaves()
        {
            repo.Delete(projects[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<Project>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void ProjectRepositoryDelete_PassNewProject_NoReturnsDbSaves()
        {
            repo.Delete(projects[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<Project>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void ProjectRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<Project>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void ProjectRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(projects[0], repo.GetCollection(project => project.Name.Contains("QA")).First());
        }

        [TestMethod]
        public void ProjectRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void ProjectRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(projects[1].Name, repo.GetEntity(2).Name);
        }

        [TestMethod]
        public void ProjectRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(projects[0].Name, repo.GetEntity(project => project.Name.StartsWith("QA")).Name);
        }

        [TestMethod]
        public void ProjectRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRWA16"));
        }
    }
}
