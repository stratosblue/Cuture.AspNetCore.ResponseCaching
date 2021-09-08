using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 执行锁
    /// </summary>
    /// <typeparam name="TCachePayload">执行锁的缓存数据</typeparam>
    public interface IExecutingLock<TCachePayload> where TCachePayload : class
    {
        #region Public 方法

        /// <inheritdoc cref="SemaphoreSlim.Release()"/>
        int Release();

        /// <summary>
        /// 设置本地缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="payload">缓存数据（传递 null 时，为清除缓存，此时 key 应当传递 <see cref="string.Empty"/>）</param>
        /// <param name="expireMilliseconds">过期时间（Unix 毫秒时间戳）</param>
        void SetLocalCache(string key, TCachePayload? payload, long expireMilliseconds);

        /// <summary>
        /// 尝试获取本地缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="checkMilliseconds">检查时间，用于与缓存有效时间进行对比（Unix 毫秒时间戳）</param>
        /// <param name="cachedPayload">缓存数据</param>
        /// <returns>是否获取到缓存</returns>
        bool TryGetLocalCache(string key, long checkMilliseconds, [NotNullWhen(true)] out TCachePayload? cachedPayload);

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int, CancellationToken)"/>
        Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken);

        #endregion Public 方法
    }
}