namespace Cuture.AspNetCore.ResponseCaching;

internal class DefaultModelKeyParser : IModelKeyParser
{
    #region Public 方法

    public string? Parse<T>(in T? model)
    {
        if (model is ICacheKeyable keyable)
        {
            return keyable.AsCacheKey();
        }
        return model?.ToString();
    }

    #endregion Public 方法
}
