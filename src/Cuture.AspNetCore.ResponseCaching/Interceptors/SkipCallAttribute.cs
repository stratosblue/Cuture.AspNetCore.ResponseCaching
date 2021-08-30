using System;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// 跳过拦截器中指定方法的调用调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SkipCallAttribute : Attribute
    {
        #region Public 构造函数

        /// <inheritdoc cref="SkipCallAttribute"/>
        public SkipCallAttribute()
        {
        }

        #endregion Public 构造函数
    }
}