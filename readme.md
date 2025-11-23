# Cuture.AspNetCore.ResponseCaching
## 1. Intro
The `asp.net core` server-side caching component implemented based on `ResourceFilter` and `ActionFilter`;

基于`ResourceFilter`和`ActionFilter`实现的`asp.net core`服务端缓存组件

## 2. 注意项
- 针对`Action`的缓存实现，基于`Filter`，非中间件/AOP实现；
- 命中缓存时直接将内容写入响应流，省略了序列化、反序列化等操作；
- 只会缓存响应内容和`ContentType`，忽略了其它响应Header；
- 支持基于`QueryKey`、`FormKey`、`Header`、`Claim`、`Model`中单个或多个组合的缓存键生成；
- 已实现基于`Memory`和`Redis`(StackExchange.Redis)的缓存，可拓展；
- 默认缓存Key生成器会包含请求路径为缓存Key；
- 默认缓存Key是大小写不敏感（强制转换为小写）的；
- `Asp.net Core`版本要求 - `8.0`以上；
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
        options.Enable = true;  //是否启用响应缓存
        options.DefaultCacheStoreLocation = CacheStoreLocation.Memory;      //默认缓存数据存储位置 - Memory
        options.DefaultExecutingLockMode = ExecutingLockMode.None;      //默认执行锁定模式 - 不锁定
        options.DefaultStrictMode = CacheKeyStrictMode.Ignore;      //默认缓存Key的严格模式 - 忽略没有的找到的Key
        options.LockedExecutionLocalResultCache = new MemoryCache(new MemoryCacheOptions());    //锁定执行时，响应的本地缓存
        options.MaxCacheableResponseLength = 1024 * 1024;       //默认最大可缓存的响应内容长度
        options.MaxCacheKeyLength = 1024;       //最大缓存Key长度
        options.OnCannotExecutionThroughLock = (cacheKey, filterContext, next) => Task.CompletedTask;     //无法使用锁执行请求时（Semaphore池用尽）的回调
        options.OnExecutionLockTimeoutFallback = (cacheKey, filterContext, next) => Task.CompletedTask;     //执行锁定超时后的处理委托
    });

    //以下为可选配置

    // 锁定执行的相关配置
    services.PostConfigure<ResponseCachingExecutingLockOptions>(options =>
    {
        options.MinimumSemaphoreRetained = 50;  //信号池的最小保留大小
        options.MaximumSemaphorePooled = 1000;  //信号池的最大大小

        options.MinimumExecutingLockRetained = 50;  //执行锁池的最小保留大小
        options.MaximumExecutingLockPooled = 1000;  //执行锁池的最大大小

        options.SemaphoreRecycleInterval = TimeSpan.FromMinutes(4);     //信号池的回收间隔
        options.ExecutingLockRecycleInterval = TimeSpan.FromMinutes(2);     //执行锁池的回收间隔
    });
}
```

### 3.3 为`Action`方法标记`ResponseCachingMetadata`Attribute与`ResponseCacheable`Attribute

#### 3.3.1 工作方式概述

 - `ResponseCachingMetadata`为派生自`IResponseCachingMetadata`的`Attribute`，用于描述响应缓存的配置等细节；
 - `ResponseCacheable`为内部实现的Attribute，用于使用`Endpoint`获取对应`Action`的`ResponseCachingMetadata`并动态构建`Filter`；

`IResponseCachingMetadata`接口及内置的`ResponseCachingMetadata`列表：
|          接口          |          描述内容          |          内置实现          |
|          ---           |            ---            |             ---           |
|`IResponseClaimCachePatternMetadata`|创建缓存时依据的 Claim 类型|`ResponseCachingAttribute`|
|`IResponseFormCachePatternMetadata`|创建缓存时依据的 Form 键|`ResponseCachingAttribute`|
|`IResponseHeaderCachePatternMetadata`|创建缓存时依据的 Header 键|`ResponseCachingAttribute`|
|`IResponseModelCachePatternMetadata`|创建缓存时依据的 Model 参数名|`ResponseCachingAttribute`|
|`IResponseQueryCachePatternMetadata`|创建缓存时依据的 Query 键|`ResponseCachingAttribute`|
|`ICacheKeyGeneratorMetadata`|用于生成缓存Key的`ICacheKeyGenerator`实现类型|`CacheKeyGeneratorAttribute`|
|`ICacheKeyStrictModeMetadata`|缓存键严格模式|`ResponseCachingAttribute`|
|`ICacheModelKeyParserMetadata`|用于生成Model的缓存Key的`IModelKeyParser`实现类型|`CacheModelKeyParserAttribute`|
|`ICacheModeMetadata`|缓存模式|`ResponseCachingAttribute`|
|`ICacheStoreLocationMetadata`|缓存数据存储位置|`ResponseCachingAttribute`|
|`IDumpStreamInitialCapacityMetadata`|Dump响应的Stream初始容量|`ResponseDumpCapacityAttribute`|
|`IExecutingLockMetadata`|`Action`的执行锁定方式|`ExecutingLockAttribute`|
|`IHotDataCacheMetadata`|热点数据缓存方式|`HotDataCacheAttribute`|
|`IMaxCacheableResponseLengthMetadata`|最大可缓存响应长度|`ResponseCachingAttribute`|
|`IResponseDurationMetadata`|缓存时长|`ResponseCachingAttribute`|

#### 3.3.2 使用内置的特性

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
                MaxCacheableResponseLength = 1024 * 1024      //最大可缓存的响应内容长度
                )]
[CacheKeyGenerator(typeof(CustomCacheKeyGenerator), FilterType.Resource)]    //使用 CustomCacheKeyGenerator 作为缓存key生成器
[CacheModelKeyParser(typeof(CustomModelKeyParser))]    //使用 CustomModelKeyParser 作为model的key解析器
[ExecutingLock(ExecutingLockMode.CacheKeySingle)]    //执行action时锁定执行过程，锁定粒度为每个缓存Key，不允许并行执行
[HotDataCache(50, HotDataCachePolicy.LRU)]    //将热点数据缓存在内存中，淘汰算法为LRU
[ResponseDumpCapacity(1024 * 1024)]    //指定dump响应的stream初始化容量，减少不必要的扩容
public IEnumerable<DataDto> Foo([FromQuery] int page, [FromQuery] int pageSize, [FromBody] RequestDto input)
{
    //...action logic
}
```

#### 3.3.3 使用自定义特性

 1. 自定义`Attribue`，继承继承`ResponseCacheableAttribute`（或继承`IFilterFactory`自行实现构建逻辑）；
 2. 自定义`Attribue`，继承需要设置的`IResponseCachingMetadata`接口；
 3. 使用上述自定义特性标记需要缓存的`Action`方法；

 - 可以使用多个`Attribute`分别实现`IResponseCachingMetadata`，也可以将所有的功能在一个`Attribute`实现；

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

### 3.5 拦截器

- 当前只有两个拦截器
    - 缓存存储拦截器：`ICacheStoringInterceptor`
    - 缓存写入拦截器：`IResponseWritingInterceptor`
- 拦截器可以有多个；
- 拦截器执行顺序为 `全局Service拦截器` -> `全局Instance拦截器` -> `Attribute拦截器`；

#### 3.5.1 自定义拦截器

 - 继承需要拦截的流程接口即可；

```C#
public class IgnoreHelloCacheStoringInterceptor
    : Attribute     //继承Attribute，以进行单个Action的设置
    , ICacheStoringInterceptor      //响应存储拦截器
{
    public async Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate<ActionContext> next)
    {
        if (key == "hello")
        {
            return null; //key为hello时，不进行存储，且不进行后续的处理
        }

        return await next(actionContext, key, entry);   //执行后续处理
    }
}
```

#### 3.5.2 设置全局拦截器

- 全局生效的拦截器

```C#
public void ConfigureServices(IServiceCollection services)
{
    //....Other Settings

    services.AddCaching().ConfigureInterceptor(options =>
    {
        //Instance拦截器
        options.AddInterceptor(new CacheHitStampInterceptor("key", "value"));    //添加拦截器 CacheHitStampInterceptor 的实例作为全局拦截器
        
        //Service拦截器
        options.AddServiceInterceptor<CustomInterceptor>();    //从DI中获取 CustomInterceptor 作为全局拦截器
    });
}
```

#### 3.5.2 设置`Action`拦截器

- 将拦截器`Attribute`设置到对应的`Action`方法即可，此时拦截器只针对当前`Action`生效

#### 3.5.3 内置的`CacheHitStamp`缓存处理拦截器

- 此拦截器将会在命中缓存时向响应的`HttpHeader`中添加指定内容
- 此设置可能因为前置拦截器短路而不执行

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

-------

## 其它

### 1. 启用 `CacheKeyAccessor` 在代码中访问当前请求的 `cache key`
```C#
services.AddCaching()
        .EnableCacheKeyAccessor(); //启用 CacheKeyAccessor ，从DI中获取 ICacheKeyAccessor 以访问当前请求的 `cache key`
```
