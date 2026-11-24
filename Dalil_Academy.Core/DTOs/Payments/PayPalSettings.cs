namespace Dalil_Academy.Core.DTOs.Payments;

public class PayPalSettings
{
    public string BaseUrl { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    public string CancelUrl { get; set; } = string.Empty;

    public decimal JodToUsdRate { get; set; } = 1.41m;
}