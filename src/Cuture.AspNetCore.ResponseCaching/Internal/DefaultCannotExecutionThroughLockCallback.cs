using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    internal static class DefaultCannotExecutionThroughLockCallback
    {
        #region Public 方法

        public static Task SetStatus429(string _, FilterContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return Task.CompletedTask;
        }

        #endregion Public 方法
    }
}