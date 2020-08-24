# Cuture.AspNetCore.ResponseCaching
## Intro
基于`ResourceFilter`和`ActionFilter`实现的`asp.net core`缓存组件

## 注意项
- 针对`Action`的缓存实现，基于`Filter`，非AOP实现；
- 命中缓存时直接将内容写入响应流，省略了序列化、反序列化等操作；
- 只会缓存响应内容和`ContentType`，忽略了其它响应Header；
- 支持基于`QueryKey`、`FormKey`、`Header`、`Claim`、`Model`中单个或多个组合的缓存键生成；
- 默认缓存Key生成器会包含请求路径为缓存Key；
- 支持基于`Memory`和`Redis`(StackExchange.Redis)的缓存；

## 使用

- 安装`Nuget`包

```PowerShell
Install-Package Cuture.AspNetCore.ResponseCaching -IncludePrerelease
```

- 在`Startup`中配置选项

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
    })
}
```

- 标记需要缓存的`Action`

使用`[ResponseCaching]`标记需要缓存响应的`Action`，或者使用简便标记`[CacheByQuery]`、`[CacheByForm]`、`[CacheByHeader]`、`[CacheByClaim]`、`[CacheByModel]`；

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
                 LockMode = ExecutingLockMode.CacheKeySingle    //执行锁定模式 - 依据缓存Key锁定（尽可能保证单机每个Key只有一个action方法体在执行）
                 )]
public IEnumerable<DataDto> Foo([FromQuery] int page, [FromQuery] int pageSize, [FromBody] RequestDto input)
{
    //...action logic
}
```

- 使用Redis进行缓存

    - 安装`Nuget`包

    ```PowerShell
    Install-Package Cuture.AspNetCore.ResponseCaching.StackExchange.Redis -IncludePrerelease
    ```

    - 配置Redis缓存

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

## 示例

### 基于Query缓存

使用query中的page和pageSize缓存
```C#
[HttpGet]
[CacheByQuery(60, "page", "pageSize")]
public ResultDto Foo(int page, int pageSize)
{
    //...action logic
}
```

### 基于Form缓存

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

### 基于用户缓存

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

## 自定义缓存实现

实现`IMemoryResponseCache`或`IDistributedResponseCache`接口；并将实现注入`asp.net core`的DI，替换掉默认实现；