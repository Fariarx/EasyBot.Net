using System;
using System.Runtime.Serialization;

namespace EasyBot
{
    class EasyBotException : Exception
    {
        public EasyBotException()
        {
        }

        public EasyBotException(string message) : base(message)
        {
        }

        public EasyBotException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EasyBotException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
