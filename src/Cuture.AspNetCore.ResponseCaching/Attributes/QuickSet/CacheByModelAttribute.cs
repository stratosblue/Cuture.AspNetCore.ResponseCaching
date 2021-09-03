using System;

using Cuture.AspNetCore.ResponseCaching.Metadatas;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 根据Model进行响应缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheByModelAttribute : ResponseCachingAttribute
    {
        #region Public 构造函数

        /// <inheritdoc cref="CacheByModelAttribute(int, CacheStoreLocation, string[])"/>
        public CacheByModelAttribute(int duration, params string[] modelNames) : base(duration, CacheMode.Custom)
        {
            if (modelNames is null)
            {
                throw new ArgumentNullException(nameof(modelNames));
            }
            VaryByModels = modelNames;
        }

        /// <summary>
        /// 根据Model进行响应缓存
        /// <para/>
        /// 具体细节参照 <see cref="IResponseModelCachePatternMetadata.VaryByModels"/>
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="storeLocation">缓存存储位置</param>
        /// <param name="modelNames">依据的model名称，不进行设置时为使用所有model进行生成</param>
        public CacheByModelAttribute(int duration, CacheStoreLocation storeLocation, params string[] modelNames) : this(duration, modelNames)
        {
            StoreLocation = storeLocation;
        }

        #endregion Public 构造函数
    }
}