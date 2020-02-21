using Microsoft.Extensions.Options;
using myAuthExampleApi.Models;
using myAuthExampleApi.Repositories;
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

        public bool IsExpired(string simpleToken)
        {
            DateTime when = GetCreationTime(simpleToken);
            return when < DateTime.UtcNow.AddMinutes(Settings.TokenExpirationInMinutes * -1);
        }

        public bool IsValid(int userId, string simpleToken)
        {
            var token = SimpleTokenRepo.Get(userId, simpleToken);
            if (token != null) SimpleTokenRepo.Delete(token);
            if (token == null || IsExpired(simpleToken)) return false;
            SimpleTokenRepo.Save();
            return true;
        }

        public void StoreToken(int userId, string simpleToken)
        {
            if (InCooldownPeriod(userId, out _)) throw new Exception($"You must wait at least {Settings.CooldownPeriodInMinutes} minutes to perform this action again");
            SimpleTokenRepo.Insert(userId, simpleToken);
            SimpleTokenRepo.Save();
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

        public ISimpleTokens GetMostRecent(int userId)
        {
            DateTime? timestamp = null;
            ISimpleTokens mostRecent = null;
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
    }
}
