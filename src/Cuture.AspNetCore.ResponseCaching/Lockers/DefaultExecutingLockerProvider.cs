using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    internal class DefaultExecutingLockerProvider : IExecutingLockerProvider
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

        public TTargetExecutingLocker GetLocker<TTargetExecutingLocker>(Type lockerType, string name)
        {
            Debug.WriteLine("Get ExecutingLocker Type：{0} - name：{1}", lockerType, name);

            if (!_lockerCreatorMap.TryGetValue(lockerType, out var lockerCreator))
            {
                throw new ResponseCachingException($"没有找到指定类型的ExecutingLockerCreator：{lockerType}");
            }

            if (lockerCreator.CreateLocker(name) is TTargetExecutingLocker executingLocker)
            {
                return executingLocker;
            }
            throw new ResponseCachingException($"无法使用类型 {lockerType} 获取到 {typeof(TTargetExecutingLocker)}");
        }

        #endregion Public 方法
    }
}