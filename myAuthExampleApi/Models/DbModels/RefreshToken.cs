using Services.JwtTokenService;

namespace myAuthExampleApi.Models.DbModels
{
    public partial class RefreshToken : IRefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
