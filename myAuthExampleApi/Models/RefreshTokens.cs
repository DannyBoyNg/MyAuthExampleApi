namespace myAuthExampleApi.Models
{
    public partial class RefreshTokens : IRefreshTokens
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
