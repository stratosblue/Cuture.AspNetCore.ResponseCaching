using Cuture.AspNetCore.ResponseCaching.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponseCaching.Test;

[TestClass]
public class QueryStringOrderUtilTest
{
    #region Public 方法

    [DataRow(10)]
    [DataRow(5)]
    [DataRow(3)]
    [DataRow(1)]
    [DataRow(0)]
    [TestMethod]
    public void Should_Order_Success(int count)
    {
        for (int i = 0; i < 1000; i++)
        {
            var kvs = Enumerable.Range(0, count).Select(m => $"{(char)('a' + m)}{Guid.NewGuid():n}={Guid.NewGuid()}").ToArray();
            var randomQuery = string.Join("&", kvs.OrderBy(_ => Guid.NewGuid()));
            var destination = new char[randomQuery.Length + 2];
            var length = QueryStringOrderUtil.Order(new Microsoft.AspNetCore.Http.QueryString('?' + randomQuery), destination);
            var output = new string(destination, 0, length);
            var target = string.Join("&", kvs);

            Assert.AreEqual(target, output.TrimEnd('&'));
        }
    }

    [TestMethod]
    public void Should_Success_With_Empty()
    {
        var destination = new char[1];
        var length = QueryStringOrderUtil.Order(new Microsoft.AspNetCore.Http.QueryString(""), destination);
        Assert.AreEqual(length, 0);
    }

    [DataRow("?v3&v1=&=v5&v4=2&_t=1&_t=0", "_t=0&_t=1&=v5&v1=&v3&v4=2&")]
    [DataRow("?v3&v1=&=v5&v4=2&_t=1&_t1=0", "_t=1&_t1=0&=v5&v1=&v3&v4=2&")]
    [DataRow("?v3&v1=&=v5&v4=2", "=v5&v1=&v3&v4=2&")]
    //----------------------------------
    [DataRow("?v3&v1&v5=", "v1&v3&v5=&")]
    [DataRow("?=v3&=v1&=v5", "=v1&=v3&=v5&")]
    [DataRow("?=v1&=v5&=v3", "=v1&=v3&=v5&")]
    //----------------------------------
    [DataRow("?v3&v1&v5=", "v1&v3&v5=&")]
    [DataRow("?v3=&v1=&v5=", "v1=&v3=&v5=&")]
    [DataRow("?v1=&v5=&v3=", "v1=&v3=&v5=&")]
    [TestMethod]
    public void Should_Success_With_NoValue_Or_NoKey(string input, string target)
    {
        var destination = new char[input.Length + 1];
        var length = QueryStringOrderUtil.Order(new Microsoft.AspNetCore.Http.QueryString(input), destination);
        var output = new string(destination, 0, length);
        Assert.AreEqual(target, output);
    }

    [TestMethod]
    public void Should_Success_With_Null()
    {
        var destination = new char[1];
        var length = QueryStringOrderUtil.Order(new Microsoft.AspNetCore.Http.QueryString(null), destination);
        Assert.AreEqual(length, 0);
    }

    [TestMethod]
    public void Should_Success_With_Query()
    {
        var destination = new char[1];
        var length = QueryStringOrderUtil.Order(new Microsoft.AspNetCore.Http.QueryString("?"), destination);
        Assert.AreEqual(length, 0);
    }

    #endregion Public 方法
}
