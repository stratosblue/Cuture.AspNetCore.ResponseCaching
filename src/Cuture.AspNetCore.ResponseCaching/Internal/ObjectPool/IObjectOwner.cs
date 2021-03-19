using System;

namespace Microsoft.Extensions.ObjectPool
{
    /// <summary>
    /// 对象所有者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectOwner<T> : IDisposable where T : class
    {
        #region Public 属性

        /// <summary>
        /// 对象
        /// </summary>
        public T Item { get; }

        #endregion Public 属性
    }
}