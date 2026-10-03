using System.ComponentModel.DataAnnotations;

namespace Forum.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите логин"), StringLength(30, MinimumLength = 3)] public string UserName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Введите email"), EmailAddress(ErrorMessage = "Некорректный email")] public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Выберите аватар")] public IFormFile? Avatar { get; set; }
    [Required(ErrorMessage = "Введите пароль"), DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    [Required, Compare(nameof(Password), ErrorMessage = "Пароли не совпадают"), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите логин или email")] public string Login { get; set; } = string.Empty;
    [Required(ErrorMessage = "Введите пароль"), DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
}
