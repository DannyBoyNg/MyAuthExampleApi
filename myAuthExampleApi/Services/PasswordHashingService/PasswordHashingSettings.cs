using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Services.PasswordHashingServ
{
    public class PasswordHashingSettings
    {
        public RandomNumberGenerator Rng { get; set; } = RandomNumberGenerator.Create();
        public KeyDerivationPrf Prf { get; set; } = KeyDerivationPrf.HMACSHA256;
        public int IterCount { get; set; } = 10000;
        public int SaltSize { get; set; } = 128 / 8;
        public int NumBytesRequested { get; set; } = 256 / 8;
    }
}
