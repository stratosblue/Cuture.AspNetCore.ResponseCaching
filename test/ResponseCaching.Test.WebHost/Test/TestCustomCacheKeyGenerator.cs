using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;

using Microsoft.AspNetCore.Mvc.Filters;

namespace ResponseCaching.Test.WebHost.Test;

public class TestCustomCacheKeyGenerator : ICacheKeyGenerator
{
    public ValueTask<string> GenerateKeyAsync(FilterContext filterContext)
    {
        var description = filterContext.ActionDescriptor.EndpointMetadata.First(m => m is DescriptionAttribute) as DescriptionAttribute;
        return new ValueTask<string>(description.Description);
    }
}
