using System.ComponentModel.DataAnnotations;

namespace Forum.Models;

public class Topic
{
    public int Id { get; set; }
    [MaxLength(150)] public string Title { get; set; } = string.Empty;
    [MaxLength(5000)] public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = null!;
    public ICollection<Reply> Replies { get; set; } = [];
}

public class Reply
{
    public int Id { get; set; }
    [MaxLength(2000)] public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = null!;
    public ICollection<ReplyVote> Votes { get; set; } = [];
}

public class ReplyVote
{
    public int ReplyId { get; set; }
    public Reply Reply { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int Value { get; set; }
}
