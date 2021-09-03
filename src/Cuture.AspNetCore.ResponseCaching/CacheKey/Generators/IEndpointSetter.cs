using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// <see cref="Endpoint"/> 设定接口
    /// </summary>
    public interface IEndpointSetter
    {
        #region Public 方法

        /// <summary>
        /// 设置 <see cref="Endpoint"/>
        /// </summary>
        /// <param name="endpoint"></param>
        void SetEndpoint(Endpoint endpoint);

        #endregion Public 方法
    }
}