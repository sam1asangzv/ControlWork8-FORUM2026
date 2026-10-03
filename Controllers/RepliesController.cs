using System.Security.Claims;
using Forum.Data;
using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

public class RepliesController(AppDbContext db) : Controller
{
    private const int PageSize = 6;

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int topicId, string content)
    {
        content = content?.Trim() ?? string.Empty;
        if (content.Length is < 1 or > 2000) return BadRequest(new { message = "Ответ должен содержать от 1 до 2000 символов" });
        if (!await db.Topics.AnyAsync(x => x.Id == topicId)) return NotFound();
        db.Replies.Add(new Reply { TopicId = topicId, Content = content, AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        var count = await db.Replies.CountAsync(x => x.TopicId == topicId);
        return Json(new { page = Math.Max(1, (int)Math.Ceiling(count / (double)PageSize)) });
    }

    [HttpGet]
    public async Task<IActionResult> Page(int topicId, int page = 1)
    {
        var topic = await db.Topics.AsNoTracking().Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == topicId);
        if (topic is null) return NotFound();
        var count = await db.Replies.CountAsync(x => x.TopicId == topicId);
        var pages = Math.Max(1, (int)Math.Ceiling(count / (double)PageSize));
        page = Math.Clamp(page, 1, pages);
        var replies = await db.Replies.AsNoTracking().Where(x => x.TopicId == topicId)
            .Include(x => x.Author).ThenInclude(x => x.Replies).Include(x => x.Votes)
            .OrderBy(x => x.CreatedAt).Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
        return PartialView("~/Views/Topics/_ReplyList.cshtml", new TopicDetailsViewModel
        {
            Topic = topic, Replies = replies, Page = page, TotalPages = pages,
            CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });
    }
}
