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
    public class TicketRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketRepository repo;
        public Mock<DbSet<Ticket>> mockSet;
        List<Ticket> Tickets;

        [TestInitialize]
        public void Setup()
        {
            Tickets = new List<Ticket>()
            {
                new Ticket("Update Question", "Updating Question Class") { Id = 1 },
                new Ticket("Failing to Update DB", "Db is not updating") { Id = 2 },
                new Ticket("Create New Feature", "Create a new feature") { Id = 3 },
                new Ticket("Late on the schedule", "Need more time to finish this") { Id = 4 },
            };

            var TicketsQueryable = Tickets.AsQueryable();
            mockSet = new Mock<DbSet<Ticket>>();
            mockSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(TicketsQueryable.Provider);
            mockSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(TicketsQueryable.Expression);
            mockSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(TicketsQueryable.ElementType);
            mockSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(TicketsQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Tickets).Returns(mockSet.Object);
            repo = new TicketRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketRepositoryAdd_PassNewTicket__NoReturnValueDbSaves()
        {
            repo.Add(new Ticket("New Test Ticket", "New test ticket's description"));

            mockSet.Verify(m => m.Add(It.IsAny<Ticket>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketRepositoryAdd_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<Ticket>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketRepositoryDelete_PassTicket__NoReturnValueDbSaves()
        {
            repo.Delete(Tickets[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<Ticket>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketRepositoryDelete_PassNewTicket_NoReturnsDbSaves()
        {
            repo.Delete(new Ticket("Delete Ticket", "New Ticket to Delete"));

            mockSet.Verify(m => m.Remove(It.IsAny<Ticket>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<Ticket>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(Tickets[0], repo.GetCollection(Ticket => Ticket.Title.Contains("Update")).First());
        }

        [TestMethod]
        public void TicketRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(Tickets[1].Title, repo.GetEntity(2).Title);
        }

        [TestMethod]
        public void TicketRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(Tickets[0].Title, repo.GetEntity(Ticket => Ticket.Title.StartsWith("Update")).Title);
        }

        [TestMethod]
        public void TicketRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRX4"));
        }
    }
}
