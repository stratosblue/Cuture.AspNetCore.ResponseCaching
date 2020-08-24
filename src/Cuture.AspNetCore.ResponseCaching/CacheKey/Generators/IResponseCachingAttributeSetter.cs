using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// <see cref="ResponseCachingAttribute"/> 设定接口
    /// </summary>
    public interface IResponseCachingAttributeSetter
    {
        /// <summary>
        /// 设置 <see cref="ResponseCachingAttribute"/>
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        void SetResponseCachingAttribute(ResponseCachingAttribute attribute);
    }
}