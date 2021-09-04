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
                for (int i = 0; i < data.Length - 1; i++)
                {
                    for (int j = i + 1; j < data.Length; j++)
                    {
                        AreNotEqual(data[i], data[j]);
                    }
                }
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

        protected virtual async Task<WeatherForecast[][]> InternalRunAsync(Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] funcs)
        {
            var tasks = new List<Task<TextHttpOperationResult<WeatherForecast[]>>>();
            foreach (var func in funcs)
            {
                var task = func();
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);

            Assert.IsFalse(tasks.Any(m => m.Result.Data == null));

            return tasks.Select(m => m.Result.Data).ToArray();
        }

        protected void AreNotEqual<T>(IEnumerable<T> items1, IEnumerable<T> items2) where T : IEquatable<T>
        {
            Assert.AreNotEqual(items1, items2);
            if (items1 != null
                && items2 != null)
            {
                var count1 = items1.Count();
                var count2 = items2.Count();

                if (count1 != count2)
                {
                    return;
                }

                using var enumerator1 = items1.GetEnumerator();
                using var enumerator2 = items2.GetEnumerator();

                bool allSame = true;
                while (enumerator1.MoveNext()
                       && enumerator2.MoveNext())
                {
                    allSame = enumerator1.Current.Equals(enumerator2.Current);
                    if (!allSame)
                    {
                        break;
                    }
                }
                Assert.AreNotEqual(true, allSame, "两个序列数据相同");
            }
        }

        protected void AreEqual<T>(IEnumerable<T> items1, IEnumerable<T> items2) where T : IEquatable<T>
        {
            if (items1 != null
                && items2 != null)
            {
                var count1 = items1.Count();
                var count2 = items2.Count();

                Assert.AreEqual(count1, count2);

                using var enumerator1 = items1.GetEnumerator();
                using var enumerator2 = items2.GetEnumerator();

                var index = 0;

                while (enumerator1.MoveNext()
                       && enumerator2.MoveNext())
                {
                    Assert.IsNotNull(enumerator1.Current);
                    Assert.IsTrue(enumerator1.Current.Equals(enumerator2.Current), $"index: {index} - Item1: {enumerator1.Current} Item2: {enumerator2.Current}");
                }
            }
        }
    }
}