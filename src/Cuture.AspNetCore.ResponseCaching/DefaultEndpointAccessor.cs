using Cuture.AspNetCore.ResponseCaching.Internal;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseCaching;

/// <inheritdoc cref="IEndpointAccessor"/>
internal class DefaultEndpointAccessor : IEndpointAccessor
{
    #region Public 属性

    /// <inheritdoc/>
    public Endpoint Endpoint { get; }

    /// <inheritdoc/>
    public EndpointMetadataCollection Metadatas => Endpoint.Metadata;

    #endregion Public 属性

    #region Public 构造函数

    public DefaultEndpointAccessor(IHttpContextAccessor httpContextAccessor)
    {
        Endpoint = httpContextAccessor?.HttpContext?.GetEndpoint() ?? throw new ResponseCachingException("Cannot access Endpoint by IHttpContextAccessor.");
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public TMetadata? GetMetadata<TMetadata>() where TMetadata : class => Endpoint.Metadata.GetMetadata<TMetadata>();

    /// <inheritdoc/>
    public TMetadata GetRequiredMetadata<TMetadata>() where TMetadata : class => Endpoint.Metadata.RequiredMetadata<TMetadata>();

    /// <inheritdoc/>
    public override string ToString() => Endpoint.ToString() ?? string.Empty;

    #endregion Public 方法
}