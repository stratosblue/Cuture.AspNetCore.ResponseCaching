# Cuture.AspNetCore.ResponseCaching
## 1. Intro
基于`ResourceFilter`和`ActionFilter`实现的`asp.net core`缓存组件

## 2. 注意项
- 针对`Action`的缓存实现，基于`Filter`，非中间件/AOP实现；
- 命中缓存时直接将内容写入响应流，省略了序列化、反序列化等操作；
- 只会缓存响应内容和`ContentType`，忽略了其它响应Header；
- 支持基于`QueryKey`、`FormKey`、`Header`、`Claim`、`Model`中单个或多个组合的缓存键生成；
- 已实现基于`Memory`和`Redis`(StackExchange.Redis)的缓存，可拓展；
- 默认缓存Key生成器会包含请求路径为缓存Key；
- `Asp.net Core`版本要求 - `3.1`以上；
- `Diagnostics`支持；
- [执行流程概览](/flow_of_execution.md)；

## 3. 如何使用

### 3.1 安装`Nuget`包

```PowerShell
Install-Package Cuture.AspNetCore.ResponseCaching
```

### 3.2 在`Startup`中配置选项

```C#
public void ConfigureServices(IServiceCollection services)
{
    //....Other Settings

    services.AddCaching(options =>
    {
        options.DefaultCacheStoreLocation = CacheStoreLocation.Memory;      //默认缓存数据存储位置 - Memory
        options.DefaultExecutingLockMode = ExecutingLockMode.None;      //默认执行锁定模式 - 不锁定
        options.DefaultStrictMode = CacheKeyStrictMode.Ignore;      //默认缓存Key的严格模式 - 忽略没有的找到的Key
        options.MaxCacheableResponseLength = 1024 * 1024;       //默认最大可缓存的响应内容长度
        options.MaxCacheKeyLength = 1024;       //最大缓存Key长度
    });
}
```

### 3.3 对需要缓存的`Action`方法进行标记

使用`[ResponseCaching]`标记需要缓存响应的`Action`，或者使用简便标记`[CacheByQuery]`、`[CacheByForm]`、`[CacheByHeader]`、`[CacheByClaim]`、`[CacheByModel]`、`[CacheByPath]`、`[CacheByFullUrl]` (这些标记都是继承自`ResponseCaching`并进行了简单的预设置)；

```C#
[ResponseCaching(
                 60,  //缓存时长（秒）
                 Mode = CacheMode.Custom,   //设置缓存模式 - 自定义缓存Key生成
                 VaryByClaims = new[] { "id" },     //依据Claim中的`id`进行构建缓存Key
                 VaryByHeaders = new[] { "version" },   //依据Header中的`version`进行构建缓存Key
                 VaryByQueryKeys = new[] { "page", "pageSize" },    //依据Query中的`page`和`pageSize`进行构建缓存Key
                 VaryByModels = new[] { "input" },  //依据Model中的`input`进行构建缓存Key
                 StoreLocation = CacheStoreLocation.Memory,     //设置缓存数据存储位置 - Memory
                 StrictMode = CacheKeyStrictMode.Ignore,    //缓存Key的严格模式 - 忽略没有的找到的Key
                 MaxCacheableResponseLength = 1024 * 1024,      //最大可缓存的响应内容长度
                 CustomCacheKeyGeneratorType = typeof(CustomCacheKeyGeneratorType),     //自定义缓存Key生成器类型
                 ModelKeyParserType = typeof(ModelKeyParserType),   //自定义Model的Key分析器
                 CachingProcessInterceptorType = typeof(CustomCachingProcessInterceptorType),   //自定义缓存处理拦截器类型
                 DumpCapacity = 1024 * 2,   //Dump响应流时的MemoryStream初始化大小
                 LockMode = ExecutingLockMode.CacheKeySingle    //执行锁定模式 - 依据缓存Key锁定（尽可能保证单机每个Key只有一个action方法体在执行）
                 )]
public IEnumerable<DataDto> Foo([FromQuery] int page, [FromQuery] int pageSize, [FromBody] RequestDto input)
{
    //...action logic
}
```

### 3.4 使用`Redis`进行缓存

不配置时将默认使用`MemoryCache`进行缓存

#### 3.4.1 安装`Nuget`包
```PowerShell
Install-Package Cuture.AspNetCore.ResponseCaching.StackExchange.Redis
```

#### 3.4.2 配置Redis缓存
```C#
public void ConfigureServices(IServiceCollection services)
{
    //....Other Settings

    services.AddCaching()
            .UseRedisResponseCache("redis:6379",    //redis配置字符串
                                    "ResponseCache_"     //缓存前缀
                                    );
}
```

### 3.5 全局拦截器的使用
当前只实现了缓存处理拦截器`ICachingProcessInterceptor`

#### 3.5.1 全局拦截器的配置
```C#
public void ConfigureServices(IServiceCollection services)
{
    //....Other Settings

    services.AddCaching().ConfigureInterceptor(options =>
    {
        options.CachingProcessInterceptorType = typeof(CustomCachingProcessInterceptorType);
    });
}
```

#### 3.5.2 内置的`CacheHitStamp`缓存处理拦截器
- 此拦截器将会在命中缓存时向响应的`HttpHeader`中添加指定内容
- 此设置可能因为拦截器短路而不执行

配置方法：
```C#
public void ConfigureServices(IServiceCollection services)
{
    //....Other Settings

    services.AddCaching().UseCacheHitStampHeader("cached", "1");    //当命中缓存时，响应的Header中将附加`cached: 1`
}
```

## 4. 示例

### 4.1 基于Query缓存

使用query中的page和pageSize缓存
```C#
[HttpGet]
[CacheByQuery(60, "page", "pageSize")]
public ResultDto Foo(int page, int pageSize)
{
    //...action logic
}
```

### 4.2 基于Form缓存

使用form中的page和pageSize缓存
```C#
[HttpGet]
[CacheByForm(60, "page", "pageSize")]
public ResultDto Foo()
{
    int page = int.Parse(Request.Form["page"]);
    int pageSize = int.Parse(Request.Form["pageSize"]);
    //...action logic
}
```

### 4.3 基于Model缓存

使用action的参数进行缓存
```C#
[HttpPost]
[CacheByModel(60)]
public ResultDto Foo(RequestDto input)
{
    //...action logic
}

public class RequestDto : ICacheKeyable
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public string AsCacheKey() => $"{Page}_{PageSize}";
}
```
使用Model缓存会使用以下方式的其中一种生成Key（优先级从上往下）
- 指定`ModelKeyParserType`
- Model类实现`ICacheKeyable`接口
- Model类的`ToString`方法

#### 此时`Filter`由`ResourceFilter`转变为`ActionFilter`

### 4.4 基于用户缓存

使用用户凭据中的`id`和query中的`page`与`pageSize`组合构建缓存Key
```C#
[ResponseCaching(60,
                 Mode = CacheMode.Custom,
                 VaryByClaims = new[] { "id" },
                 VaryByQueryKeys = new[] { "page", "pageSize" })]
public ResultDto Foo(int page, int pageSize)
{
    //...action logic
}
```

## 5. 自定义缓存实现

实现`IMemoryResponseCache`或`IDistributedResponseCache`接口；并将实现注入`asp.net core`的DI，替换掉默认实现；

-------

## Diagnostics支持

- 部分功能已使用`Diagnostic`，`DiagnosticName`为`Cuture.AspNetCore.ResponseCaching`；


事件列表如下：

|  事件名称   | 事件  |
|    ----    | ----  |
|Cuture.AspNetCore.ResponseCaching.StartProcessingCache     |开始处理缓存|
|Cuture.AspNetCore.ResponseCaching.EndProcessingCache       |处理缓存结束|
|Cuture.AspNetCore.ResponseCaching.CacheKeyGenerated        |缓存key已生成|
|Cuture.AspNetCore.ResponseCaching.ResponseFromCache        |从缓存响应请求|
|Cuture.AspNetCore.ResponseCaching.ResponseFromActionResult |使用`IActionResult`响应请求事件|
|Cuture.AspNetCore.ResponseCaching.CacheKeyTooLong          |缓存键过长|
|Cuture.AspNetCore.ResponseCaching.NoCachingFounded         |没有找到缓存|
|Cuture.AspNetCore.ResponseCaching.CacheBodyTooLong         |缓存内容过大|

-----

### 使用`ILogger`打印事件信息
- 已实现简单的`Diagnostic`订阅并打印，直接启用即可输出日志；
- 使用`Diagnostic`会对性能有那么一点影响；

#### 1. 配置服务时添加Logger
```C#
services.AddCaching()
        .AddDiagnosticDebugLogger() //在Debug模式下使用Logger输出Diagnostic事件信息
        .AddDiagnosticReleaseLogger();  //在Release模式下使用Logger输出Diagnostic事件信息
```

#### 2. 构建应用时启用Logger
```C#
app.EnableResponseCachingDiagnosticLogger();
```

如上配置后，内部将订阅相关`Diagnostic`，并将事件信息使用`ILogger`输出。