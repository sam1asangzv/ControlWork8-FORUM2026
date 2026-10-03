using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await db.Users.AnyAsync()) return;
        var anna = new ApplicationUser { UserName = "anna_forum", Email = "anna@forum.local", AvatarPath = "/images/default-avatar.svg" };
        var timur = new ApplicationUser { UserName = "timur_dev", Email = "timur@forum.local", AvatarPath = "/images/default-avatar.svg" };
        await users.CreateAsync(anna, "Forum123"); await users.CreateAsync(timur, "Forum123");
        var topics = Enumerable.Range(1, 10).Select(i => new Topic { Title = i == 1 ? "Как организовать учебный проект?" : $"Обсуждение #{i}: полезные идеи", Content = i == 1 ? "Поделитесь приемами планирования, которые помогают доводить учебные проекты до результата." : "Открытая тема для обмена опытом, вопросами и практическими советами.", AuthorId = i % 2 == 0 ? anna.Id : timur.Id, CreatedAt = DateTime.UtcNow.AddHours(-i) }).ToList();
        db.Topics.AddRange(topics); await db.SaveChangesAsync();
        for (var i = 1; i <= 14; i++) db.Replies.Add(new Reply { TopicId = topics[0].Id, AuthorId = i % 2 == 0 ? anna.Id : timur.Id, Content = $"Демонстрационный ответ {i}. Разбивайте работу на небольшие задачи и регулярно проверяйте результат.", CreatedAt = DateTime.UtcNow.AddMinutes(-15 * (15 - i)) });
        await db.SaveChangesAsync();
    }
}
