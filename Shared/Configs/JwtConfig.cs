namespace Shared.Configs
{
    public class JwtConfig
    {
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int ExpirationInDays { get; set; }
    }
}