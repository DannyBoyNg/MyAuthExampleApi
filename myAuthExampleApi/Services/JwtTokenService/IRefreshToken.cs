namespace Services.JwtTokenServ
{
    public interface IRefreshToken
    {
        string Token { get; set; }
        int UserId { get; set; }
    }
}