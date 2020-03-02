using System;
using System.Runtime.Serialization;

namespace Services.JwtTokenServ
{
    [Serializable]
    public class InvalidAccessTokenException : Exception
    {
        public InvalidAccessTokenException()
        {
        }

        public InvalidAccessTokenException(string message) : base(message)
        {
        }

        public InvalidAccessTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAccessTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}