namespace Services.JwtTokenService
{
    public interface IRefreshToken
    {
        string Token { get; set; }
        int UserId { get; set; }
    }
}