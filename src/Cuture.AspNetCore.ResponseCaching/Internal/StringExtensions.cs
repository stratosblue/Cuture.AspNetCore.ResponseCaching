using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;

internal static class StringExtensions
{
    #region Public 方法

    /// <summary>
    /// 转化为小写字符串
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static string[] ToLowerArray(this IEnumerable<string> values) => values.Select(m => m.ToLowerInvariant()).ToArray();

    /// <summary>
    /// 移除尾部的 逻辑与 符号
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static StringBuilder TrimEndAnd(this StringBuilder builder)
    {
        if (builder[^1] == '&')
        {
            builder.Length--;
        }
        return builder;
    }

    #endregion Public 方法
}
