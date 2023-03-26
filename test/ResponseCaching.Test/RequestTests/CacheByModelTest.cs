using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByModelTest : BaseRequestTest
{
    protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByModel/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 5 }).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByModel/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 6 }).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByModel/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 4 }).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByModel/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 6 }).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByModel/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 3,PageSize = 3 }).TryGetAsObjectAsync<WeatherForecast[]>(),
        };
    }
}