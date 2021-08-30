using System;
using System.Runtime.CompilerServices;

namespace Cuture.AspNetCore.ResponseCaching
{
    internal static class Checks
    {
        public static int ThrowIfDumpCapacityTooSmall(int capacity, [CallerArgumentExpression("capacity")] string? capacityExpression = null)
        {
            if (capacity < ResponseCachingConstants.DefaultMinMaxCacheableResponseLength)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{capacityExpression} can not less than {ResponseCachingConstants.DefaultMinMaxCacheableResponseLength}");
            }

            return capacity;
        }
    }
}