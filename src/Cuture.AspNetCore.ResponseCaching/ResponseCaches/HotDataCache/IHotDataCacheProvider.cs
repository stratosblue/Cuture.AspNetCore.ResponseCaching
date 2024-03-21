namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches;

/// <summary>
/// 热点数据缓存提供器
/// </summary>
public interface IHotDataCacheProvider : IDisposable
{
    #region Public 方法

    /// <summary>
    /// 获取热点数据缓存
    /// <para/>
    /// 根据 名称-策略-容量 可共享 <see cref="IHotDataCache"/>
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="name">名称</param>
    /// <param name="policy">策略</param>
    /// <param name="capacity">容量</param>
    /// <returns></returns>
    IHotDataCache Get(IServiceProvider serviceProvider, string name, HotDataCachePolicy policy, int capacity);

    #endregion Public 方法
}
