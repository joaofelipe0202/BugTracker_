namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration() 
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //Roles
            CreateRole(context, "Admin");
            CreateRole(context, "Project Manager");
            CreateRole(context, "Developer");
            CreateRole(context, "Submitter");

            //Users
            var admin1 = SeedUser(context,"Adam", "adam.bugtracker@maildrop.cc", "Mw1234.", "Admin", true);
            var admin2 = SeedUser(context, "Mike", "mike.bugtracker@maildrop.cc", "Mw1234.", "Admin", true);
            var admin3 = SeedUser(context, "Admin", "admin.bugtracker@maildrop.cc", "Mw1234.", "Admin", true);

            var pm1 = SeedUser(context, "Maggie", "maggie.bugtracker@maildrop.cc", "Mw1234.", "Project Manager", true);
            var pm2 = SeedUser(context, "Courtney", "courtney.bugtracker@maildrop.cc", "Mw1234.", "Project Manager", true);
            var pm3 = SeedUser(context, "ProjectManager", "projectmanager.bugtracker@maildrop.cc", "Mw1234.", "Project Manager", true);

            var dev1 = SeedUser(context, "Amanda", "amanda.bugtracker@maildrop.cc", "Mw1234.", "Developer", true);
            var dev2 = SeedUser(context, "Navneet", "navneet.bugtracker@maildrop.cc", "Mw1234.", "Developer", true);
            var dev3 = SeedUser(context, "Paul", "paul.bugtracker@maildrop.cc", "Mw1234.", "Developer", true);
            var dev4 = SeedUser(context, "Chris", "chris.bugtracker@maildrop.cc", "Mw1234.", "Developer", true);
            var dev5 = SeedUser(context, "Developer", "developer.bugtracker@maildrop.cc", "Mw1234.", "Developer", true);

            var subm1 = SeedUser(context, "Johnny", "johnny.bugtracker@maildrop.cc", "Mw1234.", "Submitter", true);
            var subm2 = SeedUser(context, "Maria", "maria.bugtracker@maildrop.cc", "Mw1234.", "Submitter", true);
            var subm3 = SeedUser(context, "Submitter", "submitter.bugtracker@maildrop.cc", "Mw1234.", "Submitter", true);

            //Projects
            var project1 = SeedProject(context, "QA Website");
            var project2 = SeedProject(context, "Task Management System");

            //Project Users
            var projectUser_admin1 = SeedProjectUser(context, project1.Id, admin1.Id, "Test1");
            var projectUser_pm1 = SeedProjectUser(context, project1.Id, pm1.Id, "Test2");
            var projectUser_dev1_1 = SeedProjectUser(context, project1.Id, dev1.Id, "Test3");
            var projectUser_dev1_2 = SeedProjectUser(context, project1.Id, dev2.Id, "Test4");
            var projectUser_subm1 = SeedProjectUser(context, project1.Id, subm1.Id, "Test5");

            var projectUser_admin2 = SeedProjectUser(context, project2.Id, pm2.Id, "Test6");
            var projectUser_pm2 = SeedProjectUser(context, project2.Id, pm2.Id, "Test7");
            var projectUser_dev2_1 = SeedProjectUser(context, project2.Id, dev3.Id, "Test8");
            var projectUser_dev2_2 = SeedProjectUser(context, project2.Id, dev4.Id, "Test9");
            var projectUser_subm2 = SeedProjectUser(context, project2.Id, subm2.Id, "Test10");

            var projectUser_admin3_1 = SeedProjectUser(context, project1.Id, admin3.Id, "Test11");
            var projectUser_admin3_2 = SeedProjectUser(context, project2.Id, admin3.Id, "Test12");
            var projectUser_pm3_1 = SeedProjectUser(context, project1.Id, pm3.Id, "Test13");
            var projectUser_pm3_2 = SeedProjectUser(context, project2.Id, pm3.Id, "Test14");


            //Ticket Status
            var ticketStatus1 = SeedTicketStatus(context, "Open");
            var ticketStatus2 = SeedTicketStatus(context, "In Progress");
            var ticketStatus3 = SeedTicketStatus(context, "Done");
            var ticketStatus4 = SeedTicketStatus(context, "To Do");
            var ticketStatus5 = SeedTicketStatus(context, "Modified");

            //Ticket Priority
            var ticketPriority1 = SeedTicketPriority(context, "Highest");
            var ticketPriority2 = SeedTicketPriority(context, "High");
            var ticketPriority3 = SeedTicketPriority(context, "Medium");
            var ticketPriority4 = SeedTicketPriority(context, "Low");

            //Ticket Type
            var ticketType1 = SeedTicketType(context, "Task");
            var ticketType2 = SeedTicketType(context, "Bug");
            var ticketType3 = SeedTicketType(context, "New Feature");
            var ticketType4 = SeedTicketType(context, "Problem");

            //Tickets
            var ticket1 = SeedTicket(context, "Update Question", "Updating Question Class", project1.Id, ticketType1.Id, ticketPriority3.Id, ticketStatus1.Id, subm1.Id, dev1.Id);
            var ticket2 = SeedTicket(context, "Failing to Update DB", "Db is not updating", project2.Id, ticketType2.Id, ticketPriority1.Id, ticketStatus4.Id, subm2.Id, dev3.Id);
            var ticket3 = SeedTicket(context, "Create New Feature", "Create a new feature", project1.Id, ticketType3.Id, ticketPriority2.Id, ticketStatus2.Id, subm1.Id, dev2.Id);
            var ticket4 = SeedTicket(context, "Late on the schedule", "Need more time to finish this", project2.Id, ticketType4.Id, ticketPriority4.Id, ticketStatus5.Id, subm2.Id, dev4.Id);

            //Ticket Comment
            var ticketComment1 = SeedTicketComment(context, "Search on Stack Overflow", ticket1.Id, admin1.Id);
            var ticketComment2 = SeedTicketComment(context, "Try to debug", ticket2.Id, admin2.Id);
            var ticketComment3 = SeedTicketComment(context, "We need this", ticket3.Id, pm1.Id);
            var ticketComment4 = SeedTicketComment(context, "What did happen?", ticket4.Id, dev3.Id);

            //Ticket Notification
            var ticketNotification1 = SeedTicketNotification(context, ticket1.Id, dev1.Id);
            var ticketNotification2 = SeedTicketNotification(context, ticket2.Id, dev3.Id);
            var ticketNotification3 = SeedTicketNotification(context, ticket3.Id, dev2.Id);
            var ticketNotification4 = SeedTicketNotification(context, ticket4.Id, dev4.Id);

            //Ticket History
            var ticketHistory1 = SeedTicketHistory(context, "Update", "DateTime.Now", "DateTime.Now.AddDays(20)", ticket1.Id, dev1.Id);
            var ticketHistory2 = SeedTicketHistory(context, "UserId", "2", "3", ticket2.Id, dev3.Id);
            var ticketHistory3 = SeedTicketHistory(context, "Ticket History", "To Do", "Done", ticket3.Id, dev2.Id);
            var ticketHistory4 = SeedTicketHistory(context, "TicketId", "3", "1", ticket4.Id, dev4.Id);

            //Ticket Attachment
            var ticketAttachment1 = SeedTicketAttachment(context, "MyFile.txt", "New requirements", ticket1.Id, dev1.Id, "www.bugtracker.com/MyFile.txt");
            var ticketAttachment2 = SeedTicketAttachment(context, "MyFile1.txt", "New requirements", ticket2.Id, dev3.Id, "www.bugtracker.com/MyFile1.txt");
            var ticketAttachment3 = SeedTicketAttachment(context, "MyFile2.txt", "New requirements", ticket3.Id, dev2.Id, "www.bugtracker.com/MyFile2.txt");
            var ticketAttachment4 = SeedTicketAttachment(context, "MyFile3.txt", "New requirements", ticket4.Id, dev4.Id, "www.bugtracker.com/MyFile3.txt");
        }
        private string CreateRole(ApplicationDbContext context, string roleName)
        {
            if (!context.Roles.Any(r => r.Name == roleName))
            {
                var store = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = roleName };

                roleManager.Create(role);

                return roleName;
            }

            return null;
        }
        private ApplicationUser SeedUser(ApplicationDbContext context, string userName, string email, string password, string role, bool emailConfirmed)
        {
            //UserName and Email have to be identical for now otherwise it will not work. we can fix this problem later.
            if (!context.Users.Any(u => u.UserName == userName))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = emailConfirmed,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                };
                manager.Create(user, password);

                if (role != null)
                    manager.AddToRole(user.Id, role);

                return user;
            }

            return context.Users.FirstOrDefault(user => user.UserName == userName);
        }
        private Project SeedProject(ApplicationDbContext context, string name)
        {
            Project project = new Project(name);

            context.Projects.AddOrUpdate(p => p.Name, project);
            context.SaveChanges();

            return project;
        }
        private ProjectUser SeedProjectUser(ApplicationDbContext context, int projectId, string userId, string testFlag)
        {
            ProjectUser projectUser = new ProjectUser(projectId, userId) { TestFlag = testFlag };

            context.ProjectUsers.AddOrUpdate(pu => pu.TestFlag, projectUser);
            context.SaveChanges();

            return projectUser;
        }
        private TicketStatus SeedTicketStatus(ApplicationDbContext context, string name)
        {
            TicketStatus ticketStatus = new TicketStatus(name);

            context.TicketStatuses.AddOrUpdate(ts => ts.Name, ticketStatus);
            context.SaveChanges();

            return ticketStatus;
        }
        private TicketPriority SeedTicketPriority(ApplicationDbContext context, string name)
        {
            TicketPriority ticketPriority = new TicketPriority(name);

            context.TicketPriorities.AddOrUpdate(tp => tp.Name, ticketPriority);
            context.SaveChanges();

            return ticketPriority;
        }
        private TicketType SeedTicketType(ApplicationDbContext context, string name)
        {
            TicketType ticketType = new TicketType(name);

            context.TicketTypes.AddOrUpdate(tt => tt.Name, ticketType);
            context.SaveChanges();

            return ticketType;
        }
        private Ticket SeedTicket(ApplicationDbContext context, string title, string description, int projectId, int ticketTypeId, int ticketPriorityId, int ticketStatusId, string ownerUserId, string assignedToUserId)
        {
            Ticket ticket = new Ticket
            {
                Title = title,
                Description = description,
                Created = DateTime.Now,
                Updated = DateTime.Now.AddDays(20),
                ProjectId = projectId,
                TicketTypeId = ticketTypeId,
                TicketPriorityId = ticketPriorityId,
                TicketStatusId = ticketStatusId,
                OwnerUserId = ownerUserId,
                AssignedToUserId = assignedToUserId
            };

            context.Tickets.AddOrUpdate(t => t.Title, ticket);
            context.SaveChanges();

            return ticket;
        }
        private TicketComment SeedTicketComment(ApplicationDbContext context, string comment, int ticketId, string userId)
        {
            TicketComment ticketComment = new TicketComment(comment, ticketId, userId);

            context.TicketComments.AddOrUpdate(tc => tc.Comment, ticketComment);
            context.SaveChanges();

            return ticketComment;
        }
        private TicketNotification SeedTicketNotification(ApplicationDbContext context, int ticketId, string userId)
        {
            TicketNotification ticketNotification = new TicketNotification(ticketId, userId);

            context.TicketNotifications.AddOrUpdate(tn => tn.Id, ticketNotification);
            context.SaveChanges();

            return ticketNotification;
        }
        private TicketHistory SeedTicketHistory(ApplicationDbContext context, string property, string newValue, string oldValue, int ticketId, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory(property, oldValue, newValue, ticketId, userId);

            context.TicketHistories.AddOrUpdate(th => th.Property, ticketHistory);
            context.SaveChanges();

            return ticketHistory;
        }
        private TicketAttachment SeedTicketAttachment(ApplicationDbContext context, string filePath, string description, int ticketId, string userId, string fileUrl)
        {
            TicketAttachment ticketAttachment = new TicketAttachment
            {
                FilePath = filePath,
                Description = description,
                Created = DateTime.Now,
                TicketId = ticketId,
                UserId = userId,
                FileUrl = fileUrl
            };

            context.TicketAttachments.AddOrUpdate(ta => ta.FilePath, ticketAttachment);
            context.SaveChanges();

            return ticketAttachment;
        }
    }
}
