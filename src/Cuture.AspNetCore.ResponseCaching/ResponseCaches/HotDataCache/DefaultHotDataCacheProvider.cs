using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 热点数据缓存提供器
    /// </summary>
    internal class DefaultHotDataCacheProvider : IHotDataCacheProvider
    {
        #region Private 字段

        private readonly Dictionary<string, IHotDataCache> _caches = new();
        private bool _disposedValue;

        #endregion Private 字段

        #region Public 方法

        public void Dispose()
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                IHotDataCache[] caches;

                lock (_caches)
                {
                    caches = _caches.Values.ToArray();
                    _caches.Clear();
                }

                if (caches.Length > 0)
                {
                    Task.Run(() =>
                    {
                        foreach (var item in caches)
                        {
                            item.Dispose();
                        }
                    });
                }
            }
        }

        /// <inheritdoc cref="DefaultHotDataCacheProvider"/>
        public IHotDataCache Get(IServiceProvider serviceProvider, string name, HotDataCachePolicy policy, int capacity)
        {
            name = $"{name}_{policy}_{capacity}";

            lock (_caches)
            {
                CheckDisposed();

                if (_caches.TryGetValue(name, out var cache))
                {
                    return cache;
                }
                cache = policy switch
                {
                    HotDataCachePolicy.Default => new LRUHotDataCache(capacity),
                    HotDataCachePolicy.LRU => new LRUHotDataCache(capacity),
                    _ => throw new ResponseCachingException($"UnSupport HotDataCachePolicy - {policy}."),
                };
                _caches.Add(name, cache);
                return cache;
            }
        }

        #endregion Public 方法

        #region Private 方法

        private void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(IHotDataCacheProvider));
            }
        }

        #endregion Private 方法
    }
}