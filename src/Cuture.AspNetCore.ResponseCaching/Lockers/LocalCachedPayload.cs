namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    struct LocalCachedPayload<TPayload>
    {
        public TPayload Payload;
        public long ExpireTime;
    }
}