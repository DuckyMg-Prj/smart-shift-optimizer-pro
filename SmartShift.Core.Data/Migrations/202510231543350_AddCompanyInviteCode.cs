namespace SmartShift.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyInviteCode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        UserId = c.Int(nullable: false),
                        ExpiresAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        RevokedAt = c.DateTime(),
                        ReplacedByToken = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserModels", "FailedLoginCount", c => c.Int(nullable: false));
            AddColumn("dbo.UserModels", "LockoutEndUtc", c => c.DateTime());
            AddColumn("dbo.UserModels", "LastFailedLoginUtc", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "LastFailedLoginUtc");
            DropColumn("dbo.UserModels", "LockoutEndUtc");
            DropColumn("dbo.UserModels", "FailedLoginCount");
            DropTable("dbo.RefreshTokens");
        }
    }
}
