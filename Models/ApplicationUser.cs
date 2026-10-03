using Microsoft.AspNetCore.Identity;

namespace Forum.Models;

public class ApplicationUser : IdentityUser
{
    public string AvatarPath { get; set; } = string.Empty;
    public ICollection<Topic> Topics { get; set; } = [];
    public ICollection<Reply> Replies { get; set; } = [];
}

public class UserRole
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}
