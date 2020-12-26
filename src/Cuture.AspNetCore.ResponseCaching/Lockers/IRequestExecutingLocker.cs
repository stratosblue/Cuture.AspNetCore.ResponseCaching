using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    #region Base

    /// <summary>
    /// http请求执行锁定器
    /// <para/>
    /// ！！实现 <see cref="IDisposable"/> 接口将导致Transient和Scoped实例在离开作用域时被释放
    /// </summary>
    public interface IRequestExecutingLocker<in TExecutingContext, TCachingData> where TExecutingContext : FilterContext
    {
        #region Public 方法

        /// <summary>
        /// 释放相关资源
        /// </summary>
        void Dispose();

        /// <summary>
        /// 使用锁执行并处理缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="executingContext">执行上下文</param>
        /// <param name="cacheAvailableFunc">缓存可用时的委托</param>
        /// <param name="cacheUnAvailableFunc">缓存不可用时的委托</param>
        /// <returns></returns>
        Task ProcessCacheWithLockAsync(string cacheKey, TExecutingContext executingContext, Func<TCachingData, Task> cacheAvailableFunc, Func<Task<TCachingData>> cacheUnAvailableFunc);

        #endregion Public 方法
    }

    #endregion Base

    #region Action

    #region Action Base

    /// <inheritdoc/>
    public interface IActionExecutingLocker : IRequestExecutingLocker<ActionExecutingContext, IActionResult>
    {
    }

    #endregion Action Base

    /// <inheritdoc/>
    public interface IActionSingleActionExecutingLocker : IActionExecutingLocker
    {
    }

    /// <inheritdoc/>
    public interface ICacheKeySingleActionExecutingLocker : IActionExecutingLocker
    {
    }

    #endregion Action

    #region Resource

    #region Resource Base

    /// <inheritdoc/>
    public interface IResourceExecutingLocker : IRequestExecutingLocker<ResourceExecutingContext, ResponseCacheEntry>
    {
    }

    #endregion Resource Base

    /// <inheritdoc/>
    public interface IActionSingleResourceExecutingLocker : IResourceExecutingLocker
    {
    }

    /// <inheritdoc/>
    public interface ICacheKeySingleResourceExecutingLocker : IResourceExecutingLocker
    {
    }

    #endregion Resource
}