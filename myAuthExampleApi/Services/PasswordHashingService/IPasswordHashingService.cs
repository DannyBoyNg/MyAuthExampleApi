
namespace Services.PasswordHashingServ
{
    public interface IPasswordHashingService
    {
        PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
        string HashPassword(string password);
    }
}
