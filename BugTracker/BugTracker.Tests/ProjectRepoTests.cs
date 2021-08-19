using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System;
using Moq;
using System.Collections.Generic;
using BugTracker.BL;
using BugTracker.DAL;
using BugTracker.Models;
using System.Linq;

namespace BugTracker.Tests
{
    [TestClass]
    public class ProjectRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;

        [TestInitialize]
        public void Setup()
        {
            var projectsList = new List<Project>()
            {
                new Project("QA Website"),
                new Project("Task Mangement System")
            };

            var projects = projectsList.AsQueryable();
            var projectMockSet = new Mock<DbSet<Project>>();
            projectMockSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projects.Provider);
            projectMockSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projects.Expression);
            projectMockSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projects.ElementType);
            projectMockSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projects.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Projects).Returns(projectMockSet.Object);
        }
    }
}
