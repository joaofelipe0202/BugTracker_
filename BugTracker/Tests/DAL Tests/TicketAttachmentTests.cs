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
    public class TicketAttachmentTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public TicketAttachmentRepository repo;
        public Mock<DbSet<TicketAttachment>> mockSet;
        List<TicketAttachment> TicketAttachments;

        [TestInitialize]
        public void Setup()
        {
            TicketAttachments = new List<TicketAttachment>()
            {
                new TicketAttachment("MyFile.txt", "New requirements") { Id = 1 },
                new TicketAttachment("MyFile1.txt", "New requirements") { Id = 2 },
                new TicketAttachment("MyFile2.txt", "New requirements") { Id = 3 },
                new TicketAttachment("MyFile3.txt", "New requirements") { Id = 4 },
            };

            var TicketAttachmentsQueryable = TicketAttachments.AsQueryable();
            mockSet = new Mock<DbSet<TicketAttachment>>();
            mockSet.As<IQueryable<TicketAttachment>>().Setup(m => m.Provider).Returns(TicketAttachmentsQueryable.Provider);
            mockSet.As<IQueryable<TicketAttachment>>().Setup(m => m.Expression).Returns(TicketAttachmentsQueryable.Expression);
            mockSet.As<IQueryable<TicketAttachment>>().Setup(m => m.ElementType).Returns(TicketAttachmentsQueryable.ElementType);
            mockSet.As<IQueryable<TicketAttachment>>().Setup(m => m.GetEnumerator()).Returns(TicketAttachmentsQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.TicketAttachments).Returns(mockSet.Object);
            repo = new TicketAttachmentRepository(mockContext.Object);
        }

        [TestMethod]
        public void TicketAttachmentRepositoryAdd_PassNewTicketAttachment__NoReturnValueDbSaves()
        {
            repo.Add(new TicketAttachment("NewFile.txt", "New Test Ticket Attachment "));

            mockSet.Verify(m => m.Add(It.IsAny<TicketAttachment>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketAttachmentRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockSet.Verify(m => m.Add(It.IsAny<TicketAttachment>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketAttachmentRepositoryDelete_PassTicketAttachment__NoReturnValueDbSaves()
        {
            repo.Delete(new TicketAttachment("NewTicket.txt", "New Ticket Attachment"));

            mockSet.Verify(m => m.Remove(It.IsAny<TicketAttachment>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketAttachmentRepositoryDelete_PassNewTicketAttachment_NoReturnsDbSaves()
        {
            repo.Delete(TicketAttachments[2]);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketAttachment>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TicketAttachmentRepositoryDelete_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Delete(null);

            mockSet.Verify(m => m.Remove(It.IsAny<TicketAttachment>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void TicketAttachmentRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(TicketAttachments[0], repo.GetCollection(TicketAttachment => TicketAttachment.FilePath.Contains("MyFile.txt")).First());
        }

        [TestMethod]
        public void TicketAttachmentRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void TicketAttachmentRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(TicketAttachments[1].FilePath, repo.GetEntity(2).FilePath);
        }

        [TestMethod]
        public void TicketAttachmentRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(TicketAttachments[0].FilePath, repo.GetEntity(TicketAttachment => TicketAttachment.FilePath.StartsWith("MyFile.txt")).FilePath);
        }

        [TestMethod]
        public void TicketAttachmentRepositoryGetEntity_PassString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRX4"));
        }
    }
}
