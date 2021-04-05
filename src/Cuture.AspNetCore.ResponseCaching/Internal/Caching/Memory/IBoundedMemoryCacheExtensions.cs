namespace Microsoft.Extensions.Caching.Memory
{
    /// <summary>
    ///
    /// </summary>
    internal static class IBoundedMemoryCacheExtensions
    {
        #region Public 方法

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="memoryCache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add<TKey, TValue>(this IBoundedMemoryCache<TKey, TValue> memoryCache, TKey key, TValue value) where TValue : class
        {
            memoryCache.Add(new(key, value, null));
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="memoryCache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="entryRemovingCallback"></param>
        public static void Add<TKey, TValue>(this IBoundedMemoryCache<TKey, TValue> memoryCache, TKey key, TValue value, CacheEntryRemovingCallback<TKey, TValue> entryRemovingCallback) where TValue : class
        {
            memoryCache.Add(new(key, value, entryRemovingCallback));
        }

        #endregion Public 方法
    }
}