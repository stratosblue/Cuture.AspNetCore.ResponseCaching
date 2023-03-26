using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseCaching.Internal;

internal static class EndpointMetadataCollectionExtensions
{
    #region Public 方法

    public static T RequiredMetadata<T>(this EndpointMetadataCollection metadatas) where T : class
    {
        return metadatas.GetMetadata<T>() ?? throw new ResponseCachingException($"Metadata - {typeof(T)} is required.");
    }

    #endregion Public 方法
}