using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels.Account;

public class ConfirmEmailViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Код подтверждения должен содержать 6 цифр")]
    public string ConfirmCode { get; set; }
}