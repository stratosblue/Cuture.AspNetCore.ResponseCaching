using System;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    /// <summary>
    /// 执行锁提供器
    /// </summary>
    public interface IExecutingLockerProvider
    {
        #region Public 方法

        /// <summary>
        /// 根据名称获取一个执行锁
        /// </summary>
        /// <param name="lockerType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetLocker(Type lockerType, string name);

        #endregion Public 方法
    }
}