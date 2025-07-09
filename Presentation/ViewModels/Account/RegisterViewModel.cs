// Presentation/ViewModels/Account/RegisterViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Логин обязателен")]
    [Display(Name = "Логин")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, ErrorMessage = "Пароль должен быть от {2} до {1} символов", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [Display(Name = "Подтвердите пароль")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Имя обязательно")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Фамилия обязательна")]
    [Display(Name = "Фамилия")]
    public string Surname { get; set; }

    [Display(Name = "Отчество (если есть)")]
    public string? LastName { get; set; }
}