namespace CalcOfQuantityPPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Head = c.String(),
                        ParentDepartmentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.ParentDepartmentId)
                .Index(t => t.ParentDepartmentId);
            
            CreateTable(
                "dbo.PersonalProtectiveItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProtectionClass = c.String(),
                        WearPeriod = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PPIForProfessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfessionId = c.Int(),
                        PPIId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonalProtectiveItems", t => t.PPIId)
                .ForeignKey("dbo.Professions", t => t.ProfessionId)
                .Index(t => t.ProfessionId)
                .Index(t => t.PPIId);
            
            CreateTable(
                "dbo.Professions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PPIInRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfessionsInRequestId = c.Int(),
                        PPIId = c.Int(),
                        QuantityOfPPI = c.Int(nullable: false),
                        TotalQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonalProtectiveItems", t => t.PPIId)
                .ForeignKey("dbo.ProfessionsInRequests", t => t.ProfessionsInRequestId)
                .Index(t => t.ProfessionsInRequestId)
                .Index(t => t.PPIId);
            
            CreateTable(
                "dbo.ProfessionsInRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestId = c.Int(),
                        ProfessionId = c.Int(),
                        EmployeesQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Professions", t => t.ProfessionId)
                .ForeignKey("dbo.Requests", t => t.RequestId)
                .Index(t => t.RequestId)
                .Index(t => t.ProfessionId);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        DepartmentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.ProfessionsInDepartments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(),
                        ProfessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Professions", t => t.ProfessionId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ProfessionId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProfessionsInDepartments", "ProfessionId", "dbo.Professions");
            DropForeignKey("dbo.ProfessionsInDepartments", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.PPIInRequests", "ProfessionsInRequestId", "dbo.ProfessionsInRequests");
            DropForeignKey("dbo.ProfessionsInRequests", "RequestId", "dbo.Requests");
            DropForeignKey("dbo.Requests", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.ProfessionsInRequests", "ProfessionId", "dbo.Professions");
            DropForeignKey("dbo.PPIInRequests", "PPIId", "dbo.PersonalProtectiveItems");
            DropForeignKey("dbo.PPIForProfessions", "ProfessionId", "dbo.Professions");
            DropForeignKey("dbo.PPIForProfessions", "PPIId", "dbo.PersonalProtectiveItems");
            DropForeignKey("dbo.Departments", "ParentDepartmentId", "dbo.Departments");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ProfessionsInDepartments", new[] { "ProfessionId" });
            DropIndex("dbo.ProfessionsInDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.Requests", new[] { "DepartmentId" });
            DropIndex("dbo.ProfessionsInRequests", new[] { "ProfessionId" });
            DropIndex("dbo.ProfessionsInRequests", new[] { "RequestId" });
            DropIndex("dbo.PPIInRequests", new[] { "PPIId" });
            DropIndex("dbo.PPIInRequests", new[] { "ProfessionsInRequestId" });
            DropIndex("dbo.PPIForProfessions", new[] { "PPIId" });
            DropIndex("dbo.PPIForProfessions", new[] { "ProfessionId" });
            DropIndex("dbo.Departments", new[] { "ParentDepartmentId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ProfessionsInDepartments");
            DropTable("dbo.Requests");
            DropTable("dbo.ProfessionsInRequests");
            DropTable("dbo.PPIInRequests");
            DropTable("dbo.Professions");
            DropTable("dbo.PPIForProfessions");
            DropTable("dbo.PersonalProtectiveItems");
            DropTable("dbo.Departments");
        }
    }
}
