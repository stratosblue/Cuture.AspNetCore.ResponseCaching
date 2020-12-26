using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// <see cref="ResponseCachingAttribute"/> 设定接口
    /// </summary>
    public interface IResponseCachingAttributeSetter
    {
        #region Public 方法

        /// <summary>
        /// 设置 <see cref="ResponseCachingAttribute"/>
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        void SetResponseCachingAttribute(ResponseCachingAttribute attribute);

        #endregion Public 方法
    }
}