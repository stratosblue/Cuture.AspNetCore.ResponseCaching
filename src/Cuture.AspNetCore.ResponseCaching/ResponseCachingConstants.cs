﻿namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 缓存相关常量
    /// </summary>
    public static class ResponseCachingConstants
    {
        #region const

        /// <summary>
        /// 默认字符串连接字符
        /// </summary>
        public const char CombineChar = ':';

        /// <summary>
        /// 默认dump时memorystream初始容量
        /// </summary>
        public const int DefaultDumpCapacity = 1024;

        /// <summary>
        /// 默认最大可缓存响应长度 - 512k
        /// </summary>
        public const int DefaultMaxCacheableResponseLength = 512 * 1024;

        /// <summary>
        /// 默认缓存Key的最大长度
        /// </summary>
        public const int DefaultMaxCacheKeyLength = 1024;

        /// <summary>
        /// 默认最大可缓存响应长度的最小值 - 128byte
        /// </summary>
        public const int DefaultMinMaxCacheableResponseLength = 128;

        /// <summary>
        /// 最小缓存可用毫秒数
        /// </summary>
        public const int MinCacheAvailableMilliseconds = MinCacheAvailableSeconds * 1000;

        /// <summary>
        /// 最小缓存可用秒数
        /// </summary>
        public const int MinCacheAvailableSeconds = 1;

        /// <summary>
        /// ResponseDumpContext 在Request.Items的Key
        /// </summary>
        public const string ResponseCachingResponseDumpContextKey = "__ResponseCaching.ResponseDumpContext";

        #endregion const
    }
}