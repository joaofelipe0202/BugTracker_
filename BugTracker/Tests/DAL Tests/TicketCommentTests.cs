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
    public class TicketCommentTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketCommentRepository repo;
        public Mock<DbSet<TicketComment>> mockSet;
        List<TicketComment> TicketComments;

        [TestInitialize]
        public void Setup()
        {
            TicketComments = new List<TicketComment>()
            {
                new TicketComment("Search on Stack Overflow") { Id = 1 },
                new TicketComment("Try to debug") { Id = 2 },
                new TicketComment("We need this") { Id = 3 },
                new TicketComment("What did happen?") { Id = 4 },
            };

            var TicketCommentsQueryable = TicketComments.AsQueryable();
            mockSet = new Mock<DbSet<TicketComment>>();
            mockSet.As<IQueryable<TicketComment>>().Setup(m => m.Provider).Returns(TicketCommentsQueryable.Provider);
            mockSet.As<IQueryable<TicketComment>>().Setup(m => m.Expression).Returns(TicketCommentsQueryable.Expression);
            mockSet.As<IQueryable<TicketComment>>().Setup(m => m.ElementType).Returns(TicketCommentsQueryable.ElementType);
            mockSet.As<IQueryable<TicketComment>>().Setup(m => m.GetEnumerator()).Returns(TicketCommentsQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.TicketComments).Returns(mockSet.Object);
            repo = new TicketCommentRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketCommentRepositoryAdd_PassNewTicketComment__NoReturnValueDbSaves()
        {
            repo.Add(new TicketComment("New Test TicketComment"));

            mockSet.Verify(m => m.Add(It.IsAny<TicketComment>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketCommentRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<TicketComment>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketCommentRepositoryDelete_PassTicketComment__NoReturnValueDbSaves()
        {
            repo.Delete(TicketComments[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketComment>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketCommentRepositoryDelete_PassNewTicketComment_NoReturnsDbSaves()
        {
            repo.Delete(TicketComments[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketComment>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketCommentRepositoryDelete_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketComment>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketCommentRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(TicketComments[0], repo.GetCollection(TicketComment => TicketComment.Comment.Contains("Search")).First());
        }

        [TestMethod]
        public void TicketCommentRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketCommentRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(TicketComments[1].Comment, repo.GetEntity(2).Comment);
        }

        [TestMethod]
        public void TicketCommentRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(TicketComments[0].Comment, repo.GetEntity(TicketComment => TicketComment.Comment.StartsWith("Search")).Comment);
        }

        [TestMethod]
        public void TicketCommentRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRX4"));
        }
    }
}
