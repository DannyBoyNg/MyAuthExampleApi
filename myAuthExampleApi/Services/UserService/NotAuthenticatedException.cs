using System;

namespace Services.UserServ
{
    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException()
        {
        }

        public NotAuthenticatedException(string message) : base(message)
        {
        }

        public NotAuthenticatedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
