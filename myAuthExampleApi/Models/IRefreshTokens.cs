namespace myAuthExampleApi.Models
{
    public interface IRefreshTokens
    {
        string Token { get; set; }
        int UserId { get; set; }
    }
}