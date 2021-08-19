namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestFalgToProjectUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectUsers", "TestFlag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectUsers", "TestFlag");
        }
    }
}
