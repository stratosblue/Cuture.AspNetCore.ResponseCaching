namespace Cuture.AspNetCore.ResponseCaching
{
    internal class DefaultModelKeyParser : IModelKeyParser
    {
        public string Parse(object model)
        {
            if (model is ICacheKeyable keyable)
            {
                return keyable.AsCacheKey();
            }
            return model?.ToString();
        }
    }
}