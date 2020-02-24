using Services.JwtTokenService;

namespace myAuthExampleApi.Models
{
    public partial class RefreshToken : IRefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
