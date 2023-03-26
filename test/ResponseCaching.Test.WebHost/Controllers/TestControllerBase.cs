using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Test.WebHost.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public abstract class TestControllerBase : ControllerBase
{
    #region Protected 字段

    protected const int Duration = 10;

    #endregion Protected 字段
}