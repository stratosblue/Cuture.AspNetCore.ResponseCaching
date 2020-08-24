using System;
using System.Runtime.Serialization;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseCachingException : Exception
    {
        /// <inheritdoc/>
        public ResponseCachingException()
        {
        }

        /// <inheritdoc/>
        public ResponseCachingException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public ResponseCachingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        protected ResponseCachingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}