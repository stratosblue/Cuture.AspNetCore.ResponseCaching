using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    public class DefaultExecutingLockerProvider : IExecutingLockerProvider
    {
        #region Private 字段

        private readonly Dictionary<Type, ExecutingLockerCreator> _lockerCreatorMap = new();

        #endregion Private 字段

        #region Public 构造函数

        public DefaultExecutingLockerProvider(IServiceProvider serviceProvider)
        {
            _lockerCreatorMap.Add(typeof(IActionSingleResourceExecutingLocker), new DefaultActionSingleResourceExecutingLockerCreator(serviceProvider));
            _lockerCreatorMap.Add(typeof(ICacheKeySingleResourceExecutingLocker), new DefaultResourceExecutingLockerCreator(serviceProvider));

            _lockerCreatorMap.Add(typeof(IActionSingleActionExecutingLocker), new DefaultActionSingleActionExecutingLockerCreator(serviceProvider));
            _lockerCreatorMap.Add(typeof(ICacheKeySingleActionExecutingLocker), new DefaultActionExecutingLockerCreator(serviceProvider));
        }

        #endregion Public 构造函数

        #region Public 方法

        public object GetLocker(Type lockerType, string name)
        {
            Debug.WriteLine($"Get ExecutingLocker Type：{lockerType} - name：{name}");

            if (!_lockerCreatorMap.TryGetValue(lockerType, out var lockerCreator))
            {
                throw new ResponseCachingException($"没有找到指定类型的ExecutingLockerCreator：{lockerType}");
            }

            return lockerCreator.CreateLocker(name);
        }

        #endregion Public 方法
    }
}