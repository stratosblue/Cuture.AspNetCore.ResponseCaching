using System;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 缓存依据的键没有找到
/// </summary>
[Serializable]
public class CacheVaryKeyNotFoundException : ArgumentNullException
{
    #region Public 构造函数

    /// <inheritdoc/>
    public CacheVaryKeyNotFoundException(string keyName) : base($"Cache VaryKey Not Found: {keyName}")
    {
    }

    /// <inheritdoc/>
    public CacheVaryKeyNotFoundException()
    {
    }

    /// <inheritdoc/>
    public CacheVaryKeyNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    #endregion Public 构造函数
}
