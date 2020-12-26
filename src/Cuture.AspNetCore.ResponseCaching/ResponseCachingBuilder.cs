using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// ResponseCaching构建类
    /// </summary>
    public class ResponseCachingBuilder
    {
        #region Public 属性

        /// <inheritdoc cref="IServiceCollection"/>
        public IServiceCollection Services { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// ResponseCaching构建类
        /// </summary>
        /// <param name="services"></param>
        public ResponseCachingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        #endregion Public 构造函数
    }
}