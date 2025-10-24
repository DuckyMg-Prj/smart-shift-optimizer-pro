namespace SmartShift.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShiftAssignments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShiftAssignments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShiftId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShiftRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShiftId = c.Int(nullable: false),
                        RequestedByUserId = c.Int(nullable: false),
                        TargetUserId = c.Int(),
                        Type = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShiftRequests");
            DropTable("dbo.ShiftAssignments");
        }
    }
}
