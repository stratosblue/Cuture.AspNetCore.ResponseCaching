using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.Metadatas;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 指定Model的Key解析器类型
/// <para/>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CacheModelKeyParserAttribute : Attribute, ICacheModelKeyParserMetadata
{
    #region Public 属性

    /// <inheritdoc/>
    public Type ModelKeyParserType { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="CacheModelKeyParserAttribute"/>
    /// </summary>
    /// <param name="type">
    /// Model的Key解析器类型
    /// <para/>
    /// 需要实现 <see cref="IModelKeyParser"/> 接口
    /// </param>
    public CacheModelKeyParserAttribute(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        ModelKeyParserType = Checks.ThrowIfNotIModelKeyParser(type);
    }

    #endregion Public 构造函数
}
