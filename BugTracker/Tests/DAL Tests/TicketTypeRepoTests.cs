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
    public class TicketTypeRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketTypeRepository repo;
        public Mock<DbSet<TicketType>> mockSet;
        List<TicketType> TicketTypes;

        [TestInitialize]
        public void Setup()
        {
            TicketTypes = new List<TicketType>()
            {
                new TicketType("Task") { Id = 1 },
                new TicketType("Bug") { Id = 2 },
                new TicketType("New Feature") { Id = 3 },
                new TicketType("Problem") { Id = 4 },
            };

            var TicketTypesQueryable = TicketTypes.AsQueryable();
            mockSet = new Mock<DbSet<TicketType>>();
            mockSet.As<IQueryable<TicketType>>().Setup(m => m.Provider).Returns(TicketTypesQueryable.Provider);
            mockSet.As<IQueryable<TicketType>>().Setup(m => m.Expression).Returns(TicketTypesQueryable.Expression);
            mockSet.As<IQueryable<TicketType>>().Setup(m => m.ElementType).Returns(TicketTypesQueryable.ElementType);
            mockSet.As<IQueryable<TicketType>>().Setup(m => m.GetEnumerator()).Returns(TicketTypesQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.TicketTypes).Returns(mockSet.Object);
            repo = new TicketTypeRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketTypeRepositoryAdd_PassNewTicketType__NoReturnValueDbSaves()
        {
            repo.Add(new TicketType("New Test Ticket Status"));

            mockSet.Verify(m => m.Add(It.IsAny<TicketType>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketTypeRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<TicketType>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketTypeRepositoryDelete_PassTicketType__NoReturnValueDbSaves()
        {
            repo.Delete(TicketTypes[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketType>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketTypeRepositoryDelete_PassNewTicketType_NoReturnsDbSaves()
        {
            repo.Delete(TicketTypes[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketType>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketTypeRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketType>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketTypeRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(TicketTypes[0], repo.GetCollection(TicketType => TicketType.Name.Contains("Task")).First());
        }

        [TestMethod]
        public void TicketTypeRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketTypeRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(TicketTypes[1].Name, repo.GetEntity(2).Name);
        }

        [TestMethod]
        public void TicketTypeRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(TicketTypes[0].Name, repo.GetEntity(TicketType => TicketType.Name.StartsWith("T")).Name);
        }

        [TestMethod]
        public void TicketTypeRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRWA16"));
        }
    }
}
