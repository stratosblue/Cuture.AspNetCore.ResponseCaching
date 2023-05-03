namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 有序的
/// </summary>
public interface IOrdered
{
    #region Public 属性

    /// <summary>
    /// 排序
    /// </summary>
    int Order => 1000;

    #endregion Public 属性
}
