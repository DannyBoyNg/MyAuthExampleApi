
namespace Services.PasswordHashingService
{
    public interface IPasswordHashingService
    {
        PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
        string HashPassword(string password);
    }
}
