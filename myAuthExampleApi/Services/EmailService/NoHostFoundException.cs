using System;
using System.Runtime.Serialization;

namespace Services.EmailServ
{
    [Serializable]
    public class NoHostFoundException : Exception
    {
        public NoHostFoundException()
        {
        }

        public NoHostFoundException(string message) : base(message)
        {
        }

        public NoHostFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoHostFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}