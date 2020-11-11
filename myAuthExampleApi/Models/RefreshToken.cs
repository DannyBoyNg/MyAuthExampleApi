#nullable disable

namespace myAuthExampleApi.Models
{
    public partial class RefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
