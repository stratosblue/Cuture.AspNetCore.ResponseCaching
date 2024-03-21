using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseCaching.Internal;

/// <summary>
/// QueryString排序工具
/// </summary>
internal static class QueryStringOrderUtil
{
    #region Public 方法

    public static int Order(in QueryString queryString, Span<char> destination)
    {
        if (queryString.HasValue)
        {
            if (destination.Length < queryString.Value!.Length)
            {
                throw new ArgumentException($"\"{nameof(destination)}\" not enough to fill data.");
            }

            //TODO 使用 span 优化
            var items = queryString.Value.TrimStart('?')
                                         .Split('&', StringSplitOptions.RemoveEmptyEntries)
                                         .OrderBy(m => m);

            var length = 0;
            foreach (var item in items)
            {
                var span = item.AsSpan();
                span.CopyTo(destination);
                destination[span.Length] = '&';
                destination = destination.Slice(span.Length + 1);
                length += span.Length + 1;
            }
            return length;
        }
        return 0;
    }

    #endregion Public 方法
}
