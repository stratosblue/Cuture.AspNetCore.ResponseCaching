using System;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    public interface IExecutingLockerProvider
    {
        #region Public 方法

        object GetLocker(Type lockerType, string name);

        #endregion Public 方法
    }
}