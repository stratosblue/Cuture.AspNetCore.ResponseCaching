using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CacheByCustomModelKeyParserTest : BaseRequestTest
    {
        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
                () => $"{BaseUrl}/CacheByCustomModelKeyParser/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 5 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByCustomModelKeyParser/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 6 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByCustomModelKeyParser/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 4 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByCustomModelKeyParser/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 6 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByCustomModelKeyParser/Post".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 3,PageSize = 3 }).TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }

        [TestMethod]
        public override async Task ExecuteAsync()
        {
            var funcs = GetAllRequestFuncs();

            var data = await InternalRunAsync(funcs);

            Assert.IsTrue(data.Length > 0);

            for (int i = 0; i < data.Length - 1; i++)
            {
                for (int j = i + 1; j < data.Length; j++)
                {
                    AreEqual(data[i], data[j]);
                }
            }
        }
    }
}
