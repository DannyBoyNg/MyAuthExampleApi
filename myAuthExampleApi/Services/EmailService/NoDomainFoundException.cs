using System;
using System.Runtime.Serialization;

namespace Services.EmailServ
{
    [Serializable]
    public class NoDomainFoundException : Exception
    {
        public NoDomainFoundException()
        {
        }

        public NoDomainFoundException(string message) : base(message)
        {
        }

        public NoDomainFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoDomainFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}