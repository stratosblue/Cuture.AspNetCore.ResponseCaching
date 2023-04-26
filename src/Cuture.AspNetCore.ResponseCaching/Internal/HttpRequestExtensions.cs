using System;
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
            "GET" => $"GET{CombineChar}",
            "POST" => $"POST{CombineChar}",
            "PUT" => $"PUT{CombineChar}",
            "DELETE" => $"DELETE{CombineChar}",
            "PATCH" => $"PATCH{CombineChar}",
            "OPTIONS" => $"OPTIONS{CombineChar}",
            "HEAD" => $"HEAD{CombineChar}",
            "CONNECT" => $"CONNECT{CombineChar}",
            "TRACE" => $"TRACE{CombineChar}",
            _ => $"{httpRequest.Method.ToUpperInvariant()}{CombineChar}",
        };
    }

    #endregion Public 方法
}
