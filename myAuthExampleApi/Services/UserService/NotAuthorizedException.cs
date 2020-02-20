using System;

namespace Services.UserService
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException()
        {
        }

        public NotAuthorizedException(string message) : base(message)
        {
        }

        public NotAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
