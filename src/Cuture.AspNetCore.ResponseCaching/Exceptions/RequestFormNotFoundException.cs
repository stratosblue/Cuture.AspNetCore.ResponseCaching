namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 没有找到表单
/// </summary>
[Serializable]
public class RequestFormNotFoundException : ArgumentNullException
{
    #region Public 构造函数

    /// <inheritdoc/>
    public RequestFormNotFoundException(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public RequestFormNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <inheritdoc/>
    public RequestFormNotFoundException()
    {
    }

    #endregion Public 构造函数
}
