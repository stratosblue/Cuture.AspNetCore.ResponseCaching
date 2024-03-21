using Cuture.AspNetCore.ResponseCaching;

using ResponseCaching.Test.WebHost.Dtos;

namespace ResponseCaching.Test.WebHost.Test;

public class TestCustomModelKeyParser : IModelKeyParser
{
    #region Public 方法

    public string? Parse<T>(in T? model)
    {
        if (model is PageResultRequestDto requestDto)
        {
            Console.WriteLine($"{nameof(TestCustomModelKeyParser)} for PageResultRequestDto - {requestDto.AsCacheKey()}");
            return "constant-key";
        }
        else
        {
            Console.WriteLine($"{nameof(TestCustomModelKeyParser)} for {model}");
        }
        return null;
    }

    #endregion Public 方法
}
