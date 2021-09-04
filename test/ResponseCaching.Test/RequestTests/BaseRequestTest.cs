using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.Base;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    public abstract class BaseRequestTest : WebServerHostedTestBase
    {
        protected virtual int ReRequestTimes { get; } = 3;

        /// <summary>
        /// 请求互相检查
        /// </summary>
        protected virtual bool CheckEachOtherRequest { get; } = true;

        protected abstract Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs();

        protected virtual Task BeforeRunning() => Task.CompletedTask;

        [TestMethod]
        public virtual async Task ExecuteAsync()
        {
            await BeforeRunning();

            var funcs = GetAllRequestFuncs();

            var data = await InternalRunAsync(funcs);

            Assert.IsTrue(data.Length > 0);

            if (CheckEachOtherRequest)
            {
                CheckForEachOther(data, false);
            }

            var tasks = Array(ReRequestTimes).Select(m => InternalRunAsync(funcs)).ToArray();

            await Task.WhenAll(tasks);

            var allValues = tasks.Select(m => m.Result).ToArray();

            for (int time = 0; time < ReRequestTimes; time++)
            {
                var values = allValues[time];

                Assert.AreEqual(data.Length, values.Length);

                for (int i = 0; i < values.Length; i++)
                {
                    var target = data[i];
                    var value = values[i];
                    Assert.AreEqual(target.Length, value.Length);

                    for (int index = 0; index < target.Length; index++)
                    {
                        Debug.WriteLine($"index-{index}: {target[index]} --- {value[index]}");
                        Assert.IsTrue(target[index].Equals(value[index]), "Fail at {0} , item1:{1} , item2:{2}", index, target[index], value[index]);
                    }
                }
            }
        }
    }
}