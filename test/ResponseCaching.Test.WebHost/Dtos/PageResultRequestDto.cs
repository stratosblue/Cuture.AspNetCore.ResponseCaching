using System.ComponentModel.DataAnnotations;

using Cuture.AspNetCore.ResponseCaching;

namespace ResponseCaching.Test.WebHost.Dtos;

public class PageResultRequestDto : ICacheKeyable
{
    [Range(1, 200)]
    public int Page { get; set; }

    [Range(2, 100)]
    public int PageSize { get; set; }

    public string AsCacheKey() => $"{Page}_{PageSize}";
}