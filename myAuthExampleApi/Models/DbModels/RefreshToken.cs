using Services.JwtTokenServ;

namespace myAuthExampleApi.Models.DbModels
{
    public partial class RefreshToken : IRefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
