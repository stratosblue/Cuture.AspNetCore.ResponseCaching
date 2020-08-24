using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 根据Model进行响应缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheByModelAttribute : ResponseCachingAttribute
    {
        /// <summary>
        /// 根据Model进行响应缓存
        /// <para/>
        /// 具体细节参照 <see cref="ResponseCachingAttribute.VaryByModels"/>
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="modelNames">依据的model名称，不进行设置时为使用所有model进行生成</param>
        public CacheByModelAttribute(int duration, params string[] modelNames) : base(duration)
        {
            if (modelNames is null)
            {
                throw new ArgumentNullException(nameof(modelNames));
            }
            Duration = duration;
            VaryByModels = modelNames;

            Mode = CacheMode.Custom;
        }
    }
}