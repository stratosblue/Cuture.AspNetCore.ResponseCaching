using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Filters
{
    /// <summary>
    /// 空过滤器
    /// </summary>
    internal class EmptyFilterMetadata : IFilterMetadata
    {
        #region Public 属性

        /// <summary>
        /// 静态实例
        /// </summary>
        public static EmptyFilterMetadata Instance { get; } = new EmptyFilterMetadata();

        #endregion Public 属性
    }
}