using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseCaching.Internal;

internal static class HttpRequestExtensions
{
    #region Public 方法

    public static ReadOnlySpan<char> NormalizeMethodNameAsKeyPrefix(this HttpRequest httpRequest)
    {
        const char CombineChar = ResponseCachingConstants.CombineChar;

        return httpRequest.Method switch
        {
            "GET" => $"get{CombineChar}",
            "POST" => $"post{CombineChar}",
            "PUT" => $"put{CombineChar}",
            "DELETE" => $"delete{CombineChar}",
            "PATCH" => $"patch{CombineChar}",
            "OPTIONS" => $"options{CombineChar}",
            "HEAD" => $"head{CombineChar}",
            "CONNECT" => $"connect{CombineChar}",
            "TRACE" => $"trace{CombineChar}",
            _ => $"{httpRequest.Method.ToLowerInvariant()}{CombineChar}",
        };
    }

    #endregion Public 方法
}
