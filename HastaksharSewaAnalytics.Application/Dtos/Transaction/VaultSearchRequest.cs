namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public sealed record VaultSearchRequest
{
    public string? ArmyNo { get; set; }   
    public string? Name { get; set; }      
    public string? Term { get; set; }     
}
