using BugTracker.DAL;
using BugTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class UserRepoTests
    {
        public Mock<ApplicationDbContext> mockContext;
        public UserRepository repo;
        public Mock<DbSet<ApplicationUser>> mockUserSet;
        List<ApplicationUser> ApplicationUsers;

        [TestInitialize]
        public void Setup()
        {
            ApplicationUsers = new List<ApplicationUser>() 
            {
                new ApplicationUser(){ Email = "User1@test.com", Id = "UserId1"},
                new ApplicationUser(){ Email = "User2@test.com", Id = "UserId2"},
                new ApplicationUser(){ Email = "User3@test.com", Id = "UserId3"},
                new ApplicationUser(){ Email = "User4@test.com", Id = "UserId4"},
                new ApplicationUser(){ Email = "User5@test.com", Id = "UserId5"},
                new ApplicationUser(){ Email = "User6@test.com", Id = "UserId6"},
            };

            var ApplicationUsersQueryable = ApplicationUsers.AsQueryable();
            mockUserSet = new Mock<DbSet<ApplicationUser>>();
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(ApplicationUsersQueryable.Provider);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(ApplicationUsersQueryable.Expression);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(ApplicationUsersQueryable.ElementType);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(ApplicationUsersQueryable.GetEnumerator());

            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            repo = new UserRepository(mockContext.Object);
        }

        [TestMethod]
        public void ApplicationUserRepositoryAdd_PassNewApplicationUser__NoReturnValueDbSaves()
        {
            repo.Add(new ApplicationUser() { Email="User7@test.com", Id="UserId7" });

            mockUserSet.Verify(m => m.Add(It.IsAny<ApplicationUser>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void ApplicationUserRepositoryAdd_PassNullValue_NoReturnsNoDbSaves()
        {
            repo.Add(null);

            mockUserSet.Verify(m => m.Add(It.IsAny<ApplicationUser>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void ApplicationUserRepositoryDelete_PassApplicationUser__NoReturnValueDbSaves()
        {
            repo.Delete(ApplicationUsers[2]);

            mockUserSet.Verify(m => m.Remove(It.IsAny<ApplicationUser>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void ApplicationUserRepositoryDelete_PassNewApplicationUser_NoReturnsDbSaves()
        {
            repo.Delete(ApplicationUsers[2]);

            mockUserSet.Verify(m => m.Remove(It.IsAny<ApplicationUser>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void ApplicationUserRepositoryDelete_PassNullValue_NoRetrunsNoDbSaves()
        {
            string s = null;
            repo.Delete(s);

            mockUserSet.Verify(m => m.Remove(It.IsAny<ApplicationUser>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [TestMethod]
        public void ApplicationUserRepositoryGetCollection_PassNameCondition_ReturnsCollection()
        {
            Assert.AreEqual(ApplicationUsers[0], repo.GetCollection(ApplicationUser => ApplicationUser.Email.Contains("User")).First());
        }

        [TestMethod]
        public void ApplicationUserRepositoryGetCollection_PassNullCondition_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetCollection(null));
        }

        [TestMethod]
        public void ApplicationUserRepositoryGetEntity_PassId_ReturnsEntity()
        {
            Assert.AreEqual(ApplicationUsers[1].Email, repo.GetEntity("UserId2").Email);
        }

        [TestMethod]
        public void ApplicationUserRepositoryGetEntity_PassCondition_ReturnsEntity()
        {
            Assert.AreEqual(ApplicationUsers[0].Email, repo.GetEntity(ApplicationUser => ApplicationUser.Email.StartsWith("User")).Email);
        }

        [TestMethod]
        public void ApplicationUserRepositoryGetEntity_PassInvalidString_ReturnsNull()
        {
            Assert.AreEqual(null, repo.GetEntity("QRWA16"));
        }
    }
}
