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
    public class TicketHistoryTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketHistoryRepository repo;
        public Mock<DbSet<TicketHistory>> mockSet;
        List<TicketHistory> TicketHistorys;

        [TestInitialize]
        public void Setup()
        {
            TicketHistorys = new List<TicketHistory>()
            {
                new TicketHistory("Update", "DateTime.Now", "DateTime.Now.AddDays(20)") { Id = 1 },
                new TicketHistory("UserId", "2", "3") { Id = 2 },
                new TicketHistory("Ticket History", "To Do", "Done") { Id = 3 },
                new TicketHistory("TicketId", "3", "1") { Id = 4 },
            };

            var TicketHistorysQueryable = TicketHistorys.AsQueryable();
            mockSet = new Mock<DbSet<TicketHistory>>();
            mockSet.As<IQueryable<TicketHistory>>().Setup(m => m.Provider).Returns(TicketHistorysQueryable.Provider);
            mockSet.As<IQueryable<TicketHistory>>().Setup(m => m.Expression).Returns(TicketHistorysQueryable.Expression);
            mockSet.As<IQueryable<TicketHistory>>().Setup(m => m.ElementType).Returns(TicketHistorysQueryable.ElementType);
            mockSet.As<IQueryable<TicketHistory>>().Setup(m => m.GetEnumerator()).Returns(TicketHistorysQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.TicketHistories).Returns(mockSet.Object);
            repo = new TicketHistoryRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketHistoryRepositoryAdd_PassNewTicketHistory__NoReturnValueDbSaves()
        {
            repo.Add(new TicketHistory("New Test TicketHistory", "Old Value", "New Value"));

            mockSet.Verify(m => m.Add(It.IsAny<TicketHistory>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketHistoryRepositoryAdd_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<TicketHistory>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketHistoryRepositoryDelete_PassTicketHistory__NoReturnValueDbSaves()
        {
            repo.Delete(TicketHistorys[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketHistory>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketHistoryRepositoryDelete_PassNewTicketHistory_NoReturnsDbSaves()
        {
            repo.Delete(new TicketHistory("New Ticket History to Delete", "Old value", "New value"));

            mockSet.Verify(m => m.Remove(It.IsAny<TicketHistory>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketHistoryRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketHistory>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketHistoryRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(TicketHistorys[0], repo.GetCollection(TicketHistory => TicketHistory.Property.Contains("Update")).First());
        }

        [TestMethod]
        public void TicketHistoryRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketHistoryRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(TicketHistorys[1].Property, repo.GetEntity(2).Property);
        }

        [TestMethod]
        public void TicketHistoryRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(TicketHistorys[0].Property, repo.GetEntity(TicketHistory => TicketHistory.Property.StartsWith("Update")).Property);
        }

        [TestMethod]
        public void TicketHistoryRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRX4"));
        }
    }
}
