using System;
using System.Text;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;

/// <summary>
/// 缓存键生成器
/// </summary>
public class DefaultCacheKeyGenerator : ICacheKeyGenerator
{
    #region Private 字段

    private readonly CacheKeyBuilder _innerBuilder;

    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// 缓存键生成器
    /// </summary>
    /// <param name="innerBuilder"></param>
    public DefaultCacheKeyGenerator(CacheKeyBuilder innerBuilder)
    {
        _innerBuilder = innerBuilder ?? throw new ArgumentNullException(nameof(innerBuilder));
        _stringBuilderPool = GetSharedStringBuilderPool();
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public async ValueTask<string> GenerateKeyAsync(FilterContext filterContext)
    {
        var keyBuilder = _stringBuilderPool.Get();
        try
        {
            var path = filterContext.HttpContext.Request.Path.Value!;

            if (path.EndsWith('/'))
            {
                keyBuilder.Append(path, 0, path.Length - 1);
            }
            else
            {
                keyBuilder.Append(path, 0, path.Length);
            }
            return await _innerBuilder.BuildAsync(filterContext, keyBuilder);
        }
        finally
        {
            _stringBuilderPool.Return(keyBuilder);
        }
    }

    #endregion Public 方法

    #region SharedStringBuilderPool

    private static readonly WeakReference<ObjectPool<StringBuilder>?> s_sharedStringBuilderPool = new(null, false);

    private static ObjectPool<StringBuilder> GetSharedStringBuilderPool()
    {
        if (s_sharedStringBuilderPool.TryGetTarget(out var pool))
        {
            return pool;
        }

        lock (s_sharedStringBuilderPool)
        {
            if (s_sharedStringBuilderPool.TryGetTarget(out pool))
            {
                return pool;
            }
            pool = new DefaultObjectPoolProvider() { MaximumRetained = Environment.ProcessorCount * 4 }.CreateStringBuilderPool();
            s_sharedStringBuilderPool.SetTarget(pool);
            return pool;
        }
    }

    #endregion SharedStringBuilderPool
}