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
    public class TicketStatusRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketStatusRepository repo;
        public Mock<DbSet<TicketStatus>> mockSet;
        List<TicketStatus> TicketStatuss;

        [TestInitialize]
        public void Setup()
        {
            TicketStatuss = new List<TicketStatus>()
            {
                new TicketStatus("Open") { Id = 1 },
                new TicketStatus("In Progress") { Id = 2 },
                new TicketStatus("Done") { Id = 3 },
            };

            var TicketStatussQueryable = TicketStatuss.AsQueryable();
            mockSet = new Mock<DbSet<TicketStatus>>();
            mockSet.As<IQueryable<TicketStatus>>().Setup(m => m.Provider).Returns(TicketStatussQueryable.Provider);
            mockSet.As<IQueryable<TicketStatus>>().Setup(m => m.Expression).Returns(TicketStatussQueryable.Expression);
            mockSet.As<IQueryable<TicketStatus>>().Setup(m => m.ElementType).Returns(TicketStatussQueryable.ElementType);
            mockSet.As<IQueryable<TicketStatus>>().Setup(m => m.GetEnumerator()).Returns(TicketStatussQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.TicketStatuses).Returns(mockSet.Object);
            repo = new TicketStatusRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketStatusRepositoryAdd_PassNewTicketStatus__NoReturnValueDbSaves()
        {
            repo.Add(new TicketStatus("New Test Ticket Status"));

            mockSet.Verify(m => m.Add(It.IsAny<TicketStatus>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketStatusRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<TicketStatus>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketStatusRepositoryDelete_PassTicketStatus__NoReturnValueDbSaves()
        {
            repo.Delete(TicketStatuss[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketStatus>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketStatusRepositoryDelete_PassNewTicketStatus_NoReturnsDbSaves()
        {
            repo.Delete(TicketStatuss[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketStatus>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketStatusRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketStatus>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketStatusRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(TicketStatuss[0], repo.GetCollection(TicketStatus => TicketStatus.Name.Contains("Open")).First());
        }

        [TestMethod]
        public void TicketStatusRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketStatusRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(TicketStatuss[1].Name, repo.GetEntity(2).Name);
        }

        [TestMethod]
        public void TicketStatusRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(TicketStatuss[0].Name, repo.GetEntity(TicketStatus => TicketStatus.Name.StartsWith("O")).Name);
        }

        [TestMethod]
        public void TicketStatusRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRWA16"));
        }
    }
}
