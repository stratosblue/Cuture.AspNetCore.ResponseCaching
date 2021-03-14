using System.Threading.Tasks;

using Cuture.Http;
using Cuture.Http.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponseCaching.Test.Base
{
    [TestClass]
    public abstract class AuthenticationRequiredTestBase : WebServerHostedTestBase
    {
        protected abstract Task<string> LoginAsync(string account);
    }

    [TestClass]
    public abstract class JwtAuthenticationRequiredTestBase : AuthenticationRequiredTestBase
    {
        protected override async Task<string> LoginAsync(string account)
        {
            var token = await $"{BaseUrl}/login/jwt?uid={account}".CreateHttpRequest().GetAsStringAsync();

            return token;
        }
    }

    [TestClass]
    public abstract class CookieAuthenticationRequiredTestBase : AuthenticationRequiredTestBase
    {
        protected override async Task<string> LoginAsync(string account)
        {
            var result = await $"{BaseUrl}/login/cookie?uid={account}".CreateHttpRequest().TryGetAsStringAsync();

            return CookieUtility.Clean(result.ResponseMessage.GetCookie());
        }
    }
}