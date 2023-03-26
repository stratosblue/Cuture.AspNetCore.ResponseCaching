using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 参数检查
/// </summary>
public static class Checks
{
    #region Public 方法

    /// <summary>
    /// 如果 <paramref name="capacity"/> 过小，抛出异常
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="capacityExpression"></param>
    /// <returns></returns>
    public static int ThrowIfDumpStreamInitialCapacityTooSmall(int capacity, [CallerArgumentExpression("capacity")] string? capacityExpression = null)
    {
        if (capacity < ResponseCachingConstants.DefaultMinMaxCacheableResponseLength)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), $"{capacityExpression} can not less than {ResponseCachingConstants.DefaultMinMaxCacheableResponseLength}");
        }

        return capacity;
    }

    /// <summary>
    /// 如果 <paramref name="duration"/> 过小，抛出异常
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static int ThrowIfDurationTooSmall(int duration)
    {
        if (duration < ResponseCachingConstants.MinCacheAvailableSeconds)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), $"{nameof(duration)} can not less than {ResponseCachingConstants.MinCacheAvailableSeconds} second");
        }

        return duration;
    }

    /// <summary>
    /// 如果 <paramref name="lockMode"/> 为 <see cref="ExecutingLockMode.None"/>，抛出异常
    /// </summary>
    /// <param name="lockMode"></param>
    /// <returns></returns>
    public static ExecutingLockMode ThrowIfExecutingLockModeIsNone(ExecutingLockMode lockMode)
    {
        if (lockMode == ExecutingLockMode.None)
        {
            throw new ArgumentException($"can not be \"{nameof(ExecutingLockMode.None)}\"", nameof(lockMode));
        }

        return lockMode;
    }

    /// <summary>
    /// 如果 <paramref name="lockMillisecondsTimeout"/> 无效，抛出异常
    /// </summary>
    /// <param name="lockMillisecondsTimeout"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("lockMillisecondsTimeout")]
    public static int? ThrowIfLockMillisecondsTimeoutInvalid(int? lockMillisecondsTimeout)
    {
        if (lockMillisecondsTimeout is null)
        {
            return lockMillisecondsTimeout;
        }
        if (lockMillisecondsTimeout.Value < Timeout.Infinite)
        {
            throw new ArgumentOutOfRangeException(nameof(lockMillisecondsTimeout), $"Can not less than {Timeout.Infinite}");
        }

        return lockMillisecondsTimeout;
    }

    /// <summary>
    /// 如果 <paramref name="maxCacheableResponseLength"/> 过小，抛出异常
    /// </summary>
    /// <param name="maxCacheableResponseLength"></param>
    /// <param name="variableExpression"></param>
    /// <returns></returns>
    public static int ThrowIfMaxCacheableResponseLengthTooSmall(int maxCacheableResponseLength, [CallerArgumentExpression("maxCacheableResponseLength")] string? variableExpression = null)
    {
        if (maxCacheableResponseLength < ResponseCachingConstants.DefaultMinMaxCacheableResponseLength)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCacheableResponseLength), $"Invalid maxCacheableResponseLength value - {variableExpression}");
        }

        return maxCacheableResponseLength;
    }

    /// <summary>
    /// 如果 <paramref name="cacheKeyGeneratorType"/> 不派生自 <see cref="ICacheKeyGenerator"/>，抛出异常
    /// </summary>
    /// <param name="cacheKeyGeneratorType"></param>
    /// <returns></returns>
    public static Type ThrowIfNotICacheKeyGenerator(Type cacheKeyGeneratorType)
    {
        if (!typeof(ICacheKeyGenerator).IsAssignableFrom(cacheKeyGeneratorType))
        {
            throw new ArgumentException($"CacheKeyGenerator - {cacheKeyGeneratorType} must derives from {nameof(ICacheKeyGenerator)}");
        }

        return cacheKeyGeneratorType;
    }

    /// <summary>
    /// 如果 <paramref name="modelKeyParserType"/> 不派生自 <see cref="IModelKeyParser"/>，抛出异常
    /// </summary>
    /// <param name="modelKeyParserType"></param>
    /// <returns></returns>
    public static Type ThrowIfNotIModelKeyParser(Type modelKeyParserType)
    {
        if (!typeof(IModelKeyParser).IsAssignableFrom(modelKeyParserType))
        {
            throw new ArgumentException($"ModelKeyParser - {modelKeyParserType} must derives from {nameof(IModelKeyParser)}");
        }

        return modelKeyParserType;
    }

    #endregion Public 方法
}