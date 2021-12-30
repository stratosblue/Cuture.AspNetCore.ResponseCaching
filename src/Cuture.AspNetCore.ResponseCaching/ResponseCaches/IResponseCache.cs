using System.Threading;
using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches;

/// <summary>
/// 响应缓存
/// </summary>
public interface IResponseCache
{
    #region Public 方法

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResponseCacheEntry?> GetAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetAsync(string key, ResponseCacheEntry entry, CancellationToken cancellationToken);

    #endregion Public 方法
}