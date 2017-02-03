namespace Uchat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionID = c.Int(nullable: false),
                        AnswererID = c.String(maxLength: 128),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AnswererID)
                .ForeignKey("dbo.Questions", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.QuestionID)
                .Index(t => t.AnswererID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserType = c.Int(nullable: false),
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
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        SessionID = c.Int(nullable: false),
                        StudentID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Sessions", t => t.SessionID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.StudentID)
                .Index(t => t.SessionID)
                .Index(t => t.StudentID);
            
            CreateTable(
                "dbo.QuestionLikes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionID = c.Int(nullable: false),
                        LikerID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.LikerID)
                .ForeignKey("dbo.Questions", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.QuestionID)
                .Index(t => t.LikerID);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateStarted = c.DateTime(nullable: false),
                        CourseID = c.Int(nullable: false),
                        Ended = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .Index(t => t.CourseID);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TeacherID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.TeacherID)
                .Index(t => t.TeacherID);
            
            CreateTable(
                "dbo.QuizTemplateQuestions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        CourseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .Index(t => t.CourseID);
            
            CreateTable(
                "dbo.QuizTemplateAnswers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionID = c.Int(nullable: false),
                        Text = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuizTemplateQuestions", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.QuestionID);
            
            CreateTable(
                "dbo.QuizAnswers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionID = c.Int(nullable: false),
                        Text = c.String(),
                        AnswererID = c.String(maxLength: 128),
                        IsCorrect = c.Boolean(nullable: false),
                        SessionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AnswererID)
                .ForeignKey("dbo.QuizQuestions", t => t.QuestionID, cascadeDelete: true)
                .ForeignKey("dbo.Sessions", t => t.SessionID, cascadeDelete: true)
                .Index(t => t.QuestionID)
                .Index(t => t.AnswererID)
                .Index(t => t.SessionID);
            
            CreateTable(
                "dbo.QuizQuestions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        SessionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Sessions", t => t.SessionID, cascadeDelete: false)
                .Index(t => t.SessionID);
            
            CreateTable(
                "dbo.QuizChoices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionID = c.Int(nullable: false),
                        Text = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuizQuestions", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.QuestionID);
            
            CreateTable(
                "dbo.Enrollments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CourseID = c.Int(nullable: false),
                        StudentID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.StudentID)
                .Index(t => t.CourseID)
                .Index(t => t.StudentID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Enrollments", "StudentID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Enrollments", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.Questions", "StudentID", "dbo.AspNetUsers");
            DropForeignKey("dbo.QuizAnswers", "SessionID", "dbo.Sessions");
            DropForeignKey("dbo.QuizAnswers", "QuestionID", "dbo.QuizQuestions");
            DropForeignKey("dbo.QuizQuestions", "SessionID", "dbo.Sessions");
            DropForeignKey("dbo.QuizChoices", "QuestionID", "dbo.QuizQuestions");
            DropForeignKey("dbo.QuizAnswers", "AnswererID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Questions", "SessionID", "dbo.Sessions");
            DropForeignKey("dbo.Courses", "TeacherID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Sessions", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.QuizTemplateQuestions", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.QuizTemplateAnswers", "QuestionID", "dbo.QuizTemplateQuestions");
            DropForeignKey("dbo.QuestionLikes", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.QuestionLikes", "LikerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Answers", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.Answers", "AnswererID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Enrollments", new[] { "StudentID" });
            DropIndex("dbo.Enrollments", new[] { "CourseID" });
            DropIndex("dbo.QuizChoices", new[] { "QuestionID" });
            DropIndex("dbo.QuizQuestions", new[] { "SessionID" });
            DropIndex("dbo.QuizAnswers", new[] { "SessionID" });
            DropIndex("dbo.QuizAnswers", new[] { "AnswererID" });
            DropIndex("dbo.QuizAnswers", new[] { "QuestionID" });
            DropIndex("dbo.QuizTemplateAnswers", new[] { "QuestionID" });
            DropIndex("dbo.QuizTemplateQuestions", new[] { "CourseID" });
            DropIndex("dbo.Courses", new[] { "TeacherID" });
            DropIndex("dbo.Sessions", new[] { "CourseID" });
            DropIndex("dbo.QuestionLikes", new[] { "LikerID" });
            DropIndex("dbo.QuestionLikes", new[] { "QuestionID" });
            DropIndex("dbo.Questions", new[] { "StudentID" });
            DropIndex("dbo.Questions", new[] { "SessionID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Answers", new[] { "AnswererID" });
            DropIndex("dbo.Answers", new[] { "QuestionID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Enrollments");
            DropTable("dbo.QuizChoices");
            DropTable("dbo.QuizQuestions");
            DropTable("dbo.QuizAnswers");
            DropTable("dbo.QuizTemplateAnswers");
            DropTable("dbo.QuizTemplateQuestions");
            DropTable("dbo.Courses");
            DropTable("dbo.Sessions");
            DropTable("dbo.QuestionLikes");
            DropTable("dbo.Questions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Answers");
        }
    }
}
