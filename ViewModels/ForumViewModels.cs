using Forum.Models;
using System.ComponentModel.DataAnnotations;

namespace Forum.ViewModels;

public class TopicCreateViewModel
{
    [Required(ErrorMessage = "Введите название"), StringLength(150, MinimumLength = 5)] public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Введите содержание"), StringLength(5000, MinimumLength = 10)] public string Content { get; set; } = string.Empty;
}

public class TopicListViewModel
{
    public required IReadOnlyList<Topic> Topics { get; init; }
    public int Page { get; init; }
    public int TotalPages { get; init; }
}

public class TopicDetailsViewModel
{
    public required Topic Topic { get; init; }
    public required IReadOnlyList<Reply> Replies { get; init; }
    public int Page { get; init; }
    public int TotalPages { get; init; }
    public string? CurrentUserId { get; init; }
}
