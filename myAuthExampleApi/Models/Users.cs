namespace myAuthExampleApi.Models
{
    public partial class Users : IUsers
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Active { get; set; }
    }
}
