namespace Microsoft.Extensions.ObjectPool
{
    /// <summary>
    /// 池缩减策略
    /// </summary>
    internal interface IPoolReductionPolicy
    {
        #region Public 属性

        /// <summary>
        /// 默认测策略
        /// </summary>
        public static IPoolReductionPolicy Default { get; } = new DefaultPoolReductionPolicy();

        #endregion Public 属性

        #region Public 方法

        /// <summary>
        /// 计算缩减对象池后，池保留对象的数量
        /// </summary>
        /// <param name="currentPoolSize">当前对象池大小</param>
        /// <param name="poolMinimumRetained">对象池应该保留的对象数量</param>
        /// <returns>池应该缩减到的大小</returns>
        int NextSize(int currentPoolSize, int poolMinimumRetained);

        #endregion Public 方法
    }
}