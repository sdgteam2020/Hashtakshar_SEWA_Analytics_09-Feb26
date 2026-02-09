namespace HastaksharSewaAnalytics.Application.Dtos.User;

public sealed class RegisterVm
{
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
}


