using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 动态构建 Filter 时 <see cref="Endpoint"/> 访问器
    /// </summary>
    public interface IEndpointAccessor
    {
        #region Public 属性

        /// <inheritdoc cref="Microsoft.AspNetCore.Http.Endpoint"/>
        Endpoint Endpoint { get; }

        /// <inheritdoc cref="EndpointMetadataCollection"/>
        EndpointMetadataCollection Metadatas { get; }

        #endregion Public 属性

        #region Public 方法

        /// <summary>
        /// 尝试获取 <typeparamref name="TMetadata"/>
        /// </summary>
        /// <typeparam name="TMetadata"></typeparam>
        /// <returns></returns>
        TMetadata? GetMetadata<TMetadata>() where TMetadata : class;

        /// <summary>
        /// 获取 <typeparamref name="TMetadata"/>，如不存在，则抛出异常
        /// </summary>
        /// <typeparam name="TMetadata"></typeparam>
        /// <returns></returns>
        TMetadata GetRequiredMetadata<TMetadata>() where TMetadata : class;

        #endregion Public 方法
    }
}