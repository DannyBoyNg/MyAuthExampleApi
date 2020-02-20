namespace myAuthExampleApi.Models
{
    public interface IUsers
    {
        bool Active { get; set; }
        string Email { get; set; }
        bool EmailConfirmed { get; set; }
        int Id { get; set; }
        string PasswordHash { get; set; }
        string UserName { get; set; }
    }
}