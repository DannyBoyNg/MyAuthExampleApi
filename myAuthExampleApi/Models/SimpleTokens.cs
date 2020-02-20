namespace myAuthExampleApi.Models
{
    public partial class SimpleTokens : ISimpleTokens
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
