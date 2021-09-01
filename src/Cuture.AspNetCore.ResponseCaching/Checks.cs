using System;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching
{
    internal static class Checks
    {
        #region Public 方法

        public static int ThrowIfDumpCapacityTooSmall(int capacity, [CallerArgumentExpression("capacity")] string? capacityExpression = null)
        {
            if (capacity < ResponseCachingConstants.DefaultMinMaxCacheableResponseLength)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{capacityExpression} can not less than {ResponseCachingConstants.DefaultMinMaxCacheableResponseLength}");
            }

            return capacity;
        }

        public static ExecutingLockMode ThrowIfExecutingLockModeIsNone(ExecutingLockMode lockMode)
        {
            if (lockMode == ExecutingLockMode.None)
            {
                throw new ArgumentException($"can not be \"{nameof(ExecutingLockMode.None)}\"", nameof(lockMode));
            }

            return lockMode;
        }

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
}