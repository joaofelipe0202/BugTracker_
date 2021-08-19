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
    public class TicketPrioritiesRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketPriorityRepository repo;
        public Mock<DbSet<TicketPriority>> mockSet;
        List<TicketPriority> TicketPrioritys;

        [TestInitialize]
        public void Setup()
        {
            TicketPrioritys = new List<TicketPriority>()
            {
                new TicketPriority("Highest") { Id = 1 },
                new TicketPriority("High") { Id = 2 },
                new TicketPriority("Medium") { Id = 3 },
                 new TicketPriority("Low") { Id = 4 },
            };

            var TicketPrioritysQueryable = TicketPrioritys.AsQueryable();
            mockSet = new Mock<DbSet<TicketPriority>>();
            mockSet.As<IQueryable<TicketPriority>>().Setup(m => m.Provider).Returns(TicketPrioritysQueryable.Provider);
            mockSet.As<IQueryable<TicketPriority>>().Setup(m => m.Expression).Returns(TicketPrioritysQueryable.Expression);
            mockSet.As<IQueryable<TicketPriority>>().Setup(m => m.ElementType).Returns(TicketPrioritysQueryable.ElementType);
            mockSet.As<IQueryable<TicketPriority>>().Setup(m => m.GetEnumerator()).Returns(TicketPrioritysQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.TicketPriorities).Returns(mockSet.Object);
            repo = new TicketPriorityRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketPriorityRepositoryAdd_PassNewTicketPriority__NoReturnValueDbSaves()
        {
            repo.Add(new TicketPriority("New Test Ticket Status"));

            mockSet.Verify(m => m.Add(It.IsAny<TicketPriority>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketPriorityRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<TicketPriority>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketPriorityRepositoryDelete_PassTicketPriority__NoReturnValueDbSaves()
        {
            repo.Delete(TicketPrioritys[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketPriority>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketPriorityRepositoryDelete_PassNewTicketPriority_NoReturnsDbSaves()
        {
            repo.Delete(TicketPrioritys[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketPriority>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketPriorityRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketPriority>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketPriorityRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(TicketPrioritys[0], repo.GetCollection(TicketPriority => TicketPriority.Name.Contains("High")).First());
        }

        [TestMethod]
        public void TicketPriorityRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketPriorityRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(TicketPrioritys[1].Name, repo.GetEntity(2).Name);
        }

        [TestMethod]
        public void TicketPriorityRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(TicketPrioritys[0].Name, repo.GetEntity(TicketPriority => TicketPriority.Name.StartsWith("H")).Name);
        }

        [TestMethod]
        public void TicketPriorityRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRWA16"));
        }
    }
}
