using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.Base;

namespace ResponseCaching.Test.RequestTests;

public abstract class RequestTestBase : WebServerHostedTestBase
{
    #region Public 方法

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="requestFucFactors">请求创建委托列表</param>
    /// <param name="checkEachOtherRequest">请求结果互相检查</param>
    /// <param name="shouldEqualEachOtherRequest"><paramref name="checkEachOtherRequest"/> 时检查相同还是不同</param>
    /// <param name="reRequestTimes">重复请求次数</param>
    /// <param name="reRequestShouldEqual">重复请求响应应该相同</param>
    /// <returns></returns>
    protected async Task ExecuteAsync<TResult>(Func<Task<TextHttpOperationResult<TResult[]>>>[] requestFucFactors,
                                               bool checkEachOtherRequest = true,
                                               bool shouldEqualEachOtherRequest = false,
                                               int reRequestTimes = 3,
                                               bool reRequestShouldEqual = true)
        where TResult : IEquatable<TResult>
    {
        var funcs = requestFucFactors;
        var data = await InternalRunAsync(funcs);

        Assert.IsTrue(data.Length > 0);

        if (checkEachOtherRequest)
        {
            CheckForEachOther(data, shouldEqualEachOtherRequest);
        }

        var tasks = Array(reRequestTimes).Select(m => InternalRunAsync(funcs)).ToArray();

        await Task.WhenAll(tasks);

        var allValues = tasks.Select(m => m.Result).ToArray();

        //对比同一请求多次请求的结果是否一致
        for (int time = 0; time < reRequestTimes; time++)
        {
            var values = allValues[time];

            if (reRequestShouldEqual)
            {
                Assert.AreEqual(data.Length, values.Length);
            }

            for (int i = 0; i < values.Length; i++)
            {
                var target = data[i];
                var value = values[i];

                if (reRequestShouldEqual)
                {
                    Assert.AreEqual(target.Length, value.Length);
                }

                for (int index = 0; index < target.Length; index++)
                {
                    Debug.WriteLine($"index-{index}: {target[index]} --- {value[index]}");

                    if (reRequestShouldEqual)
                    {
                        Assert.IsTrue(target[index].Equals(value[index]), "Fail at {0} , item1:{1} , item2:{2}", index, target[index], value[index]);
                    }
                    else
                    {
                        Assert.IsFalse(target[index].Equals(value[index]), "Fail at {0} , item1:{1} , item2:{2}", index, target[index], value[index]);
                    }
                }
            }
        }
    }

    #endregion Public 方法
}
