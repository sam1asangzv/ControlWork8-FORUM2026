using Forum.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

public class ProfileController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Details(string id)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user is null) return NotFound();
        ViewBag.MessageCount = await db.Replies.CountAsync(x => x.AuthorId == id);
        ViewBag.TopicCount = await db.Topics.CountAsync(x => x.AuthorId == id);
        return View(user);
    }
}
