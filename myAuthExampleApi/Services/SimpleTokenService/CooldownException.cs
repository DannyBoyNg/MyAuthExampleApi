using System;
using System.Runtime.Serialization;

namespace Services.SimpleTokenServ
{
    [Serializable]
    public class CooldownException : Exception
    {
        public TimeSpan? CooldownLeft { get; set; }

        public CooldownException()
        {
        }

        public CooldownException(string message) : base(message)
        {
        }

        public CooldownException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CooldownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}