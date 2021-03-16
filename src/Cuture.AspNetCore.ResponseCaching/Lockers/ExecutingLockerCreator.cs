using System.Collections.Generic;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    internal class DefaultActionExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultActionExecutingLocker();
        }

        #endregion Protected 方法
    }

    internal class DefaultActionSingleActionExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultActionSingleActionExecutingLocker();
        }

        #endregion Protected 方法
    }

    internal class DefaultActionSingleResourceExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultActionSingleResourceExecutingLocker();
        }

        #endregion Protected 方法
    }

    internal class DefaultResourceExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultResourceExecutingLocker();
        }

        #endregion Protected 方法
    }

    internal abstract class ExecutingLockerCreator
    {
        #region Private 字段

        private readonly Dictionary<string, object> _lockers = new();

        #endregion Private 字段

        #region Public 方法

        public object CreateLocker(string name)
        {
            object? cachedLocker;
            lock (_lockers)
            {
                if (!_lockers.TryGetValue(name, out cachedLocker))
                {
                    cachedLocker = CreateLocker();
                    _lockers.Add(name, cachedLocker);
                }
            }
            return cachedLocker;
        }

        #endregion Public 方法

        #region Protected 方法

        protected abstract object CreateLocker();

        #endregion Protected 方法
    }
}