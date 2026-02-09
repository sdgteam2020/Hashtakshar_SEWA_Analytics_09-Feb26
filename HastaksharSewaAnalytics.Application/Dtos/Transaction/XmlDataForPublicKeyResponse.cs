namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public sealed class XmlDataForPublicKeyResponse
{
    public string? Public_Key { get; set; }
    public string? SerialNo { get; set; }

    public bool Status { get; set; } = true;     // optional: set true when found
    public bool TokenValid { get; set; }

    public string? ValidFrom { get; set; }
    public string? ValidTo { get; set; }
}
