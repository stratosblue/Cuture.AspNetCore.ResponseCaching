using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// ResponseCaching构建类
    /// </summary>
    public class ResponseCachingBuilder
    {
        /// <inheritdoc cref="IServiceCollection"/>
        public IServiceCollection Services { get; }

        /// <summary>
        /// ResponseCaching构建类
        /// </summary>
        /// <param name="services"></param>
        public ResponseCachingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}