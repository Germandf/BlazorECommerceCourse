using System.ComponentModel.DataAnnotations;

namespace BlazorECommerceCourse.Shared;
public class UserRegister
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";
    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = "";
    [Required, Compare(nameof(Password), ErrorMessage = "The passwords do not match")]
    public string ConfirmPassword { get; set; } = "";
}
