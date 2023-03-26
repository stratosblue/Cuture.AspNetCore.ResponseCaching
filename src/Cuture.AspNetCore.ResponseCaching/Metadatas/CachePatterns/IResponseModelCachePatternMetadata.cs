using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachePatternMetadata"/> - 基于 Model 的缓存
/// </summary>
public interface IResponseModelCachePatternMetadata : IResponseCachePatternMetadata
{
    #region Public 属性

    /// <summary>
    /// 创建缓存时依据的 Model 参数名
    /// <para/>
    /// Note:
    /// <para/>
    /// * 以下为使用默认实现时的情况
    /// <para/>
    /// * 使用空数组时为使用所有 Model 进行生成Key
    /// <para/>
    /// * 使用的Filter将会从 <see cref="IAsyncResourceFilter"/> 转变为 <see cref="IAsyncActionFilter"/>
    /// <para/>
    /// * 由于内部的实现问题，<see cref="IExecutingLockMetadata.LockMode"/> 的设置在某些情况下可能无法严格限制所有请求
    /// <para/>
    /// * 生成Key时，如果没有指定 <see cref="ICacheModelKeyParserMetadata.ModelKeyParserType"/>，
    /// 则检查Model是否实现 <see cref="ICacheKeyable"/> 接口，如果Model未实现 <see cref="ICacheKeyable"/> 接口，
    /// 则调用Model的 <see cref="object.ToString"/> 方法生成Key
    /// </summary>
    string[]? VaryByModels { get; }

    #endregion Public 属性
}