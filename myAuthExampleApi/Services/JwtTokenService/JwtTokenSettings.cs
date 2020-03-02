namespace Services.JwtTokenServ
{
    public class JwtTokenSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpirationInMinutes { get; set; } = 60;
        public int RefreshTokenExpirationInHours { get; set; } = 1;
    }
}
