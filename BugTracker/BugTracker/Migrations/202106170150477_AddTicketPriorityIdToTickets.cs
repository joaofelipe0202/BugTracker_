namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketPriorityIdToTickets : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Tickets", "TicketPriorityId");
            AddForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities");
            DropIndex("dbo.Tickets", new[] { "TicketPriorityId" });
        }
    }
}
