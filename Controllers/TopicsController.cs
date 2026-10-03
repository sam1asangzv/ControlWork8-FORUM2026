using System.Security.Claims;
using Forum.Data;
using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

public class TopicsController(AppDbContext db) : Controller
{
    private const int PageSize = 8;

    public async Task<IActionResult> Index(int page = 1)
    {
        page = Math.Max(page, 1);
        var query = db.Topics.AsNoTracking().Include(x => x.Author).Include(x => x.Replies).OrderByDescending(x => x.CreatedAt);
        var count = await query.CountAsync();
        var pages = Math.Max(1, (int)Math.Ceiling(count / (double)PageSize));
        page = Math.Min(page, pages);
        var topics = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
        return View(new TopicListViewModel { Topics = topics, Page = page, TotalPages = pages });
    }

    [Authorize, HttpGet] public IActionResult Create() => View(new TopicCreateViewModel());

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TopicCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var topic = new Topic
        {
            Title = model.Title.Trim(), Content = model.Content.Trim(),
            AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!, CreatedAt = DateTime.UtcNow
        };
        db.Topics.Add(topic);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = topic.Id });
    }

    public async Task<IActionResult> Details(int id, int page = 1)
    {
        var topic = await db.Topics.AsNoTracking().Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
        if (topic is null) return NotFound();
        const int replyPageSize = 6;
        var count = await db.Replies.CountAsync(x => x.TopicId == id);
        var pages = Math.Max(1, (int)Math.Ceiling(count / (double)replyPageSize));
        page = Math.Clamp(page, 1, pages);
        var replies = await db.Replies.Where(x => x.TopicId == id)
            .Include(x => x.Author).ThenInclude(x => x.Replies).Include(x => x.Votes).OrderBy(x => x.CreatedAt)
            .Skip((page - 1) * replyPageSize).Take(replyPageSize).ToListAsync();
        return View(new TopicDetailsViewModel { Topic = topic, Replies = replies, Page = page, TotalPages = pages, CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
    }
}
