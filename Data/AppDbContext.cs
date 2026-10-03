using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<IdentityRole> Roles => Set<IdentityRole>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Reply> Replies => Set<Reply>();
    public DbSet<ReplyVote> ReplyVotes => Set<ReplyVote>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ApplicationUser>().ToTable("Users").HasIndex(x => x.NormalizedUserName).IsUnique();
        b.Entity<ApplicationUser>().HasIndex(x => x.NormalizedEmail).IsUnique();
        b.Entity<IdentityRole>().ToTable("Roles").HasIndex(x => x.NormalizedName).IsUnique();
        b.Entity<UserRole>().ToTable("UserRoles").HasKey(x => new { x.UserId, x.RoleId });
        b.Entity<Topic>().HasOne(x => x.Author).WithMany(x => x.Topics).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Reply>().HasOne(x => x.Author).WithMany(x => x.Replies).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ReplyVote>().HasKey(x => new { x.ReplyId, x.UserId });
        b.Entity<ReplyVote>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
