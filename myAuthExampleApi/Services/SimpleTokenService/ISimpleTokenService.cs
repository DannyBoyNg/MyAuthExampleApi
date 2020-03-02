using System;

namespace Services.SimpleTokenServ
{
    public interface ISimpleTokenService
    {
        SimpleTokenSettings Settings { get; set; }

        string Generate();
        DateTime GetCreationTime(string simpleToken);
        void StoreToken(int userId, string simpleToken);
        void Validate(int userId, string simpleToken);
    }
}