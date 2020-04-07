using Ng.Services;

namespace myAuthExampleApi.Models.DbModels
{
    public partial class SimpleToken : ISimpleToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
