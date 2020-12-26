using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// 默认拦截器配置
    /// </summary>
    public class InterceptorOptions : IOptions<InterceptorOptions>
    {
        #region Private 字段

        private Type _cachingProcessInterceptorType;

        #endregion Private 字段

        #region Public 属性

        /// <summary>
        /// 缓存处理拦截器类型
        /// <para/>
        /// 需要实现 <see cref="ICachingProcessInterceptor"/> 接口
        /// </summary>
        public Type CachingProcessInterceptorType
        {
            get => _cachingProcessInterceptorType;
            set => CheckAndSetType(ref _cachingProcessInterceptorType, value, typeof(ICachingProcessInterceptor));
        }

        /// <summary>
        ///
        /// </summary>
        public InterceptorOptions Value => this;

        #endregion Public 属性

        #region Private 方法

        private static void CheckAndSetType(ref Type target, Type value, Type baseType)
        {
            if (value is null || value.IsSubclassOf(baseType))
            {
                target = value;
            }
            throw new ResponseCachingException($"{value.FullName} is not subclass of {baseType.FullName}");
        }

        #endregion Private 方法
    }
}