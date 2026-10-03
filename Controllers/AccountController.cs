using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers;

public class AccountController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, IWebHostEnvironment env) : Controller
{
    [HttpGet] public IActionResult Login() => View(new LoginViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = model.Login.Contains('@') ? await users.FindByEmailAsync(model.Login) : await users.FindByNameAsync(model.Login);
        if (user is not null && (await signIn.PasswordSignInAsync(user, model.Password, true, false)).Succeeded)
            return RedirectToAction("Index", "Topics");
        ModelState.AddModelError(string.Empty, "Неверный логин, email или пароль");
        return View(model);
    }

    [HttpGet] public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model.Avatar is not null && (!model.Avatar.ContentType.StartsWith("image/") || model.Avatar.Length > 3 * 1024 * 1024))
            ModelState.AddModelError(nameof(model.Avatar), "Выберите изображение размером до 3 МБ");
        if (!ModelState.IsValid) return View(model);
        var extension = Path.GetExtension(model.Avatar!.FileName).ToLowerInvariant();
        if (extension is not ".jpg" and not ".jpeg" and not ".png" and not ".webp")
        {
            ModelState.AddModelError(nameof(model.Avatar), "Разрешены JPG, PNG и WEBP");
            return View(model);
        }
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = $"/uploads/avatars/{fileName}";
        await using (var stream = System.IO.File.Create(Path.Combine(env.WebRootPath, "uploads", "avatars", fileName)))
            await model.Avatar.CopyToAsync(stream);
        var user = new ApplicationUser { UserName = model.UserName.Trim(), Email = model.Email.Trim(), AvatarPath = relativePath };
        var result = await users.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await signIn.SignInAsync(user, true);
            return RedirectToAction("Index", "Topics");
        }
        System.IO.File.Delete(Path.Combine(env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, Translate(error.Code));
        return View(model);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() { await signIn.SignOutAsync(); return RedirectToAction("Index", "Topics"); }

    public IActionResult AccessDenied() => View();

    private static string Translate(string code) => code switch
    {
        "DuplicateUserName" => "Этот логин уже занят",
        "DuplicateEmail" => "Этот email уже зарегистрирован",
        "PasswordTooShort" => "Пароль должен содержать минимум 6 символов",
        "PasswordRequiresUpper" => "Добавьте заглавную букву",
        "PasswordRequiresLower" => "Добавьте строчную букву",
        "PasswordRequiresDigit" => "Добавьте цифру",
        _ => "Не удалось создать пользователя"
    };
}
