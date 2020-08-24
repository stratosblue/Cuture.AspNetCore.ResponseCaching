namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// Model缓存key解析器
    /// </summary>
    public interface IModelKeyParser
    {
        /// <summary>
        /// 解析为key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string Parse(object model);
    }
}