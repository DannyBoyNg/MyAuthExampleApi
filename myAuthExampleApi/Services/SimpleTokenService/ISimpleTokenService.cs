using myAuthExampleApi.Models;
using System;

namespace Services.SimpleTokenService
{
    public interface ISimpleTokenService
    {
        SimpleTokenSettings Settings { get; set; }

        string Generate();
        DateTime GetCreationTime(string simpleToken);
        ISimpleTokens GetMostRecent(int userId);
        void StoreToken(int userId, string simpleToken);
        bool IsExpired(string simpleToken);
        bool ValidateToken(int userId, string simpleToken);
    }
}