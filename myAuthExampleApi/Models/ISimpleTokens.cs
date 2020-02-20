namespace myAuthExampleApi.Models
{
    public interface ISimpleTokens
    {
        string Token { get; set; }
        int UserId { get; set; }
    }
}