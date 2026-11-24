
namespace Dalil_Academy.Infra.Security;

public class JwtSetting
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public double ExpiresInHours { get; set; } = 0.02;
}
