namespace SmartShift.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShiftEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(),
                        OwnerUserId = c.Int(),
                        StartUtc = c.DateTime(nullable: false),
                        EndUtc = c.DateTime(nullable: false),
                        RequiredStaff = c.Int(nullable: false),
                        IsOffDay = c.Boolean(nullable: false),
                        ClientId = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shifts");
        }
    }
}
