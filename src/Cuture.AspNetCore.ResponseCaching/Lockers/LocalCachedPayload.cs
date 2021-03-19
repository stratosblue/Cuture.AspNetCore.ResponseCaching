namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// 本地缓存的响应数据
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    internal struct LocalCachedPayload<TPayload>
    {
        #region Public 字段

        public long ExpireTime;
        public TPayload Payload;

        #endregion Public 字段
    }
}