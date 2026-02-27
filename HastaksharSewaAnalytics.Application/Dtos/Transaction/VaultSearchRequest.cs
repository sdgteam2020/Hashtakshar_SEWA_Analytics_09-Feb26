namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public sealed record VaultSearchRequest
{
    public string? ArmyNo { get; set; }   // you are using this as "term"
    public string? Name { get; set; }     // kept for future
    public string? Term { get; set; }     // optional
}
