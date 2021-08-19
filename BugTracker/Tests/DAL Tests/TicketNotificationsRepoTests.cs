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
    public class TicketNotificationsRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketNotificationRepository repo;
        public Mock<DbSet<TicketNotification>> mockSet;
        List<TicketNotification> TicketNotifications;

        [TestInitialize]
        public void Setup()
        {
            TicketNotifications = new List<TicketNotification>()
            {
                new TicketNotification(1, "User-Id-1") { Id = 1 },
                new TicketNotification(2, "User-Id-2") { Id = 2 },
                new TicketNotification(3, "User-Id-3") { Id = 3 },
            };

            var TicketNotificationsQueryable = TicketNotifications.AsQueryable();
            mockSet = new Mock<DbSet<TicketNotification>>();
            mockSet.As<IQueryable<TicketNotification>>().Setup(m => m.Provider).Returns(TicketNotificationsQueryable.Provider);
            mockSet.As<IQueryable<TicketNotification>>().Setup(m => m.Expression).Returns(TicketNotificationsQueryable.Expression);
            mockSet.As<IQueryable<TicketNotification>>().Setup(m => m.ElementType).Returns(TicketNotificationsQueryable.ElementType);
            mockSet.As<IQueryable<TicketNotification>>().Setup(m => m.GetEnumerator()).Returns(TicketNotificationsQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.TicketNotifications).Returns(mockSet.Object);
            repo = new TicketNotificationRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketNotificationRepositoryAdd_PassNewTicketNotification__NoReturnValueDbSaves()
        {
            repo.Add(new TicketNotification(4, "User-Id-4"));

            mockSet.Verify(m => m.Add(It.IsAny<TicketNotification>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketNotificationRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<TicketNotification>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketNotificationRepositoryDelete_PassTicketNotification__NoReturnValueDbSaves()
        {
            repo.Delete(TicketNotifications[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketNotification>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketNotificationRepositoryDelete_PassNewTicketNotification_NoReturnsDbSaves()
        {
            repo.Delete(TicketNotifications[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketNotification>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketNotificationRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketNotification>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketNotificationRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(TicketNotifications[1].TicketId, repo.GetCollection(TicketNotification => TicketNotification.TicketId == 2).First().TicketId);
        }

        [TestMethod]
        public void TicketNotificationRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketNotificationRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(TicketNotifications[1].TicketId, repo.GetEntity(2).TicketId);
        }

        [TestMethod]
        public void TicketNotificationRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(TicketNotifications[0].TicketId, repo.GetEntity(TicketNotification => TicketNotification.TicketId == 1).TicketId);
        }

        [TestMethod]
        public void TicketNotificationRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRWA16"));
        }
    }
}
