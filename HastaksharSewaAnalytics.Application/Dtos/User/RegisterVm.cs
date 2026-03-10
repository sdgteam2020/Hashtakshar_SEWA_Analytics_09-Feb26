using System.ComponentModel.DataAnnotations;

namespace HastaksharSewaAnalytics.Application.Dtos.User;

public sealed class RegisterVm
{
    [Required]
    [MinLength(4)]
    [MaxLength(15)]
    public string Username { get; set; } = "";

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = "";

    [Required]
    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
    public string ConfirmPassword { get; set; } = "";
}


public sealed class RegisterEncryptedVm
{
    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    [Required]
    public string ConfirmPassword { get; set; } = "";
}