using System;
using System.Runtime.Serialization;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
///
/// </summary>
public class ResponseCachingException : Exception
{
    #region Public 构造函数

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

    #endregion Public 构造函数

    #region Protected 构造函数

    /// <inheritdoc/>
    protected ResponseCachingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    #endregion Protected 构造函数
}