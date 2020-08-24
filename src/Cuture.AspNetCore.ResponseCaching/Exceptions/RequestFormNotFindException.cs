using System;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 没有找到表单
    /// </summary>
    public class RequestFormNotFindException : ArgumentNullException
    {
        /// <inheritdoc/>
        public RequestFormNotFindException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public RequestFormNotFindException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        public RequestFormNotFindException()
        {
        }
    }
}