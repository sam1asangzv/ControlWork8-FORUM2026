using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace Forum.Migrations;

[Migration("202610030001_InitialCreate")]
public class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder m)
    {
        m.Sql("""
        CREATE TABLE Users (Id TEXT PRIMARY KEY, UserName TEXT, NormalizedUserName TEXT, Email TEXT, NormalizedEmail TEXT, EmailConfirmed INTEGER NOT NULL, PasswordHash TEXT, SecurityStamp TEXT, ConcurrencyStamp TEXT, PhoneNumber TEXT, PhoneNumberConfirmed INTEGER NOT NULL, TwoFactorEnabled INTEGER NOT NULL, LockoutEnd TEXT, LockoutEnabled INTEGER NOT NULL, AccessFailedCount INTEGER NOT NULL, AvatarPath TEXT NOT NULL);
        CREATE UNIQUE INDEX IX_Users_NormalizedUserName ON Users(NormalizedUserName);
        CREATE UNIQUE INDEX IX_Users_NormalizedEmail ON Users(NormalizedEmail);
        CREATE TABLE Roles (Id TEXT PRIMARY KEY, Name TEXT, NormalizedName TEXT, ConcurrencyStamp TEXT);
        CREATE UNIQUE INDEX IX_Roles_NormalizedName ON Roles(NormalizedName);
        CREATE TABLE UserRoles (UserId TEXT NOT NULL, RoleId TEXT NOT NULL, PRIMARY KEY(UserId, RoleId));
        CREATE TABLE Topics (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT NOT NULL, Content TEXT NOT NULL, CreatedAt TEXT NOT NULL, AuthorId TEXT NOT NULL, FOREIGN KEY(AuthorId) REFERENCES Users(Id) ON DELETE RESTRICT);
        CREATE INDEX IX_Topics_AuthorId ON Topics(AuthorId);
        CREATE TABLE Replies (Id INTEGER PRIMARY KEY AUTOINCREMENT, Content TEXT NOT NULL, CreatedAt TEXT NOT NULL, TopicId INTEGER NOT NULL, AuthorId TEXT NOT NULL, FOREIGN KEY(TopicId) REFERENCES Topics(Id) ON DELETE CASCADE, FOREIGN KEY(AuthorId) REFERENCES Users(Id) ON DELETE RESTRICT);
        CREATE INDEX IX_Replies_TopicId ON Replies(TopicId);
        CREATE INDEX IX_Replies_AuthorId ON Replies(AuthorId);
        CREATE TABLE ReplyVotes (ReplyId INTEGER NOT NULL, UserId TEXT NOT NULL, Value INTEGER NOT NULL, PRIMARY KEY(ReplyId, UserId), FOREIGN KEY(ReplyId) REFERENCES Replies(Id) ON DELETE CASCADE, FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE RESTRICT);
        CREATE INDEX IX_ReplyVotes_UserId ON ReplyVotes(UserId);
        """);
    }

    protected override void Down(MigrationBuilder m) => m.Sql("DROP TABLE ReplyVotes; DROP TABLE Replies; DROP TABLE Topics; DROP TABLE UserRoles; DROP TABLE Roles; DROP TABLE Users;");
}
