namespace Microsoft.Extensions.ObjectPool
{
    //HACK 目前定义为内部接口，不允许外部手动返还对象。是否该放宽限制

    /// <summary>
    /// 回收池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IRecyclePool<T> where T : class
    {
        #region Public 方法

        /// <summary>
        /// 将对象还回对象池
        /// </summary>
        /// <param name="item"></param>
        void Return(T item);

        #endregion Public 方法
    }
}