using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Services.SimpleTokenService
{
    public class SimpleTokenService : ISimpleTokenService
    {
        public SimpleTokenSettings Settings { get; set; }
        ISimpleTokenRepository SimpleTokenRepo { get; }

        public SimpleTokenService(
            IOptions<SimpleTokenSettings> options,
            ISimpleTokenRepository simpleTokenRepo)
        {
            Settings = options.Value;
            SimpleTokenRepo = simpleTokenRepo;
        }

        public string Generate()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public void Validate(int userId, string simpleToken)
        {
            var tokenExpired = false;
            var tokens = SimpleTokenRepo.GetByUserId(userId);
            //Remove expired simple tokens from db
            foreach (var token in tokens)
            {
                if (IsExpired(token.Token))
                {
                    if (token.Token == simpleToken) tokenExpired = true;
                    SimpleTokenRepo.Delete(token);
                }
            }
            //Validate user provided simple token
            var dbToken = tokens.Where(x => x.Token == simpleToken).SingleOrDefault();
            if (dbToken != null) SimpleTokenRepo.Delete(dbToken);
            SimpleTokenRepo.Save();
            if (tokenExpired) throw new Exception("Token expired");
            if (dbToken == null) throw new Exception("Invalid token");
        }

        public void StoreToken(int userId, string simpleToken)
        {
            if (InCooldownPeriod(userId, out TimeSpan? cooldownLeft)) throw new CooldownException($"You must wait at least {Settings.CooldownPeriodInMinutes} minutes to perform this action again") { CooldownLeft = cooldownLeft };
            SimpleTokenRepo.Insert(userId, simpleToken);
            SimpleTokenRepo.Save();
        }

        public DateTime GetCreationTime(string simpleToken)
        {
            if (simpleToken == null) throw new ArgumentNullException(nameof(simpleToken));
            simpleToken = simpleToken.Replace('_', '/').Replace('-', '+');
            switch (simpleToken.Length % 4)
            {
                case 2: simpleToken += "=="; break;
                case 3: simpleToken += "="; break;
            }
            byte[] data = Convert.FromBase64String(simpleToken);
            return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        }

        private bool InCooldownPeriod(int userId, out TimeSpan? cooldownLeft)
        {
            var token = GetMostRecent(userId);
            if (token == null)
            {
                cooldownLeft = null;
                return false;
            }
            var tokenCreationTime = GetCreationTime(token.Token);
            cooldownLeft = tokenCreationTime - DateTime.UtcNow.AddMinutes(-1 * Settings.CooldownPeriodInMinutes);
            return tokenCreationTime > DateTime.UtcNow.AddMinutes(-1 * Settings.CooldownPeriodInMinutes);
        }

        private ISimpleToken GetMostRecent(int userId)
        {
            DateTime? timestamp = null;
            ISimpleToken mostRecent = null;
            var tokens = SimpleTokenRepo.GetByUserId(userId).ToList();
            foreach (var token in tokens)
            {
                if (IsExpired(token.Token))
                {
                    SimpleTokenRepo.Delete(token);
                }
                var creation = GetCreationTime(token.Token);
                if (timestamp == null || creation > timestamp)
                {
                    timestamp = creation;
                    mostRecent = token;
                }
            }
            return mostRecent;
        }

        private bool IsExpired(string simpleToken)
        {
            if (Settings.TokenExpirationInMinutes == 0) return false; //When set to 0, simple token never expires
            DateTime when = GetCreationTime(simpleToken);
            return when < DateTime.UtcNow.AddMinutes(Settings.TokenExpirationInMinutes * -1);
        }
    }
}
