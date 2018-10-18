using System;
using System.Runtime.Serialization;

namespace EasyBot.Imaging
{
    class ImagingException : Exception
    {
        public ImagingException()
        {
        }

        public ImagingException(string message) : base(message)
        {
        }

        public ImagingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImagingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
