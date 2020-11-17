using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.Base;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public abstract class BaseRequestTest : WebServerHostedTestBase
    {
        protected abstract Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs();

        protected virtual Task BeforeRunning() => Task.CompletedTask;

        [TestMethod]
        public virtual async Task ExecuteAsync()
        {
            await BeforeRunning();

            var funcs = GetAllRequestFuncs();

            var data = await IntervalRunAsync(funcs);

            Assert.IsTrue(data.Length > 0);

            for (int i = 0; i < data.Length - 1; i++)
            {
                for (int j = i + 1; j < data.Length; j++)
                {
                    Assert.AreNotEqual(data[i], data[j]);
                }
            }

            for (int time = 0; time < 3; time++)
            {
                var values = await IntervalRunAsync(funcs);

                Assert.AreEqual(data.Length, values.Length);

                for (int i = 0; i < values.Length; i++)
                {
                    var target = data[i];
                    var value = values[i];
                    Assert.AreEqual(target.Length, value.Length);

                    for (int index = 0; index < target.Length; index++)
                    {
                        Debug.WriteLine($"index-{index}: {target[index]} --- {value[index]}");
                        Assert.IsTrue(target[index].Equals(value[index]));
                    }
                }
            }
        }

        protected virtual async Task<WeatherForecast[][]> IntervalRunAsync(Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] funcs)
        {
            var tasks = new List<Task<TextHttpOperationResult<WeatherForecast[]>>>();
            foreach (var func in funcs)
            {
                var task = func();
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);

            //if (tasks.Any(m => m.Result.Data == null))
            //{
            //}

            return tasks.Select(m => m.Result.Data).ToArray();
        }
    }
}