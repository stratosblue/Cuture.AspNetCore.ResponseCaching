namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// Model缓存key解析器
/// </summary>
public interface IModelKeyParser
{
    #region Public 方法

    /// <summary>
    /// 解析为key
    /// </summary>
    /// <param name="model"></param>
    /// <returns><paramref name="model"/>解析后等价的key字符串</returns>
    string? Parse(object? model);

    #endregion Public 方法
}
