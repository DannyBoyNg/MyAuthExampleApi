namespace Services.SimpleTokenServ
{
    public interface ISimpleToken
    {
        string Token { get; set; }
        int UserId { get; set; }
    }
}