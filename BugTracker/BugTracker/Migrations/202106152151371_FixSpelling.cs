namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSpelling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketHistories", "OldValue", c => c.String());
            DropColumn("dbo.TicketHistories", "OldVlaue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketHistories", "OldVlaue", c => c.String());
            DropColumn("dbo.TicketHistories", "OldValue");
        }
    }
}
