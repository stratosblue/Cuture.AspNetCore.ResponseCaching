# 部分执行流程

使用支持mermaid的Markdown编辑器查看

-----

## 1. Filter构建流程
```mermaid
graph TD 
    请求 --asp.net core 请求处理管道--> 
    FilterFactory["ResponseCachingAttribute(IFilterFactory)"] --开始构建Filter-->
    B1["从DI中获取IOptions<ResponseCachingOptions>"] -->
    B2["依据选项从DI中获取IResponseCache"] -->
    B3["依据选项确定ICacheKeyGenerator"] -->
    B4["从DI获取IResponseCacheDeterminer"] -->
    B5["确定使用的IRequestExecutingLocker及ResponseCachingContext"] --> 
    B6["创建Filter并返回"] --> Filter构建结束
```

----

### 1.1. IResponseCache选择流程
```mermaid
graph TD 
    B2["依据选项从DI中获取IResponseCache"]
        B2 --Distributed--> IDistributedResponseCache
        B2 --Memory--> IMemoryResponseCache
```

----

### 1.2. ICacheKeyGenerator选择流程
```mermaid
graph TD 
    B3["依据选项确定ICacheKeyGenerator"]
    B3 --设置了CustomCacheKeyGeneratorType--> B3_1["从DI中获取CustomCacheKeyGeneratorType作为CacheKeyGenerator"]
    B3 --未设置CustomCacheKeyGeneratorType--> B3_2["依据CacheMode选择CacheKeyGenerator"]
        B3_2 --FullPathAndQuery--> 从DI获取FullPathAndQueryCacheKeyGenerator
        B3_2 --Custom--> 依据设置的VaryBy*属性构建CacheKeyGenerator
        B3_2 --PathUniqueness--> 从DI获取RequestPathCacheKeyGenerator
```

----

### 1.3. IRequestExecutingLocker及ResponseCachingContext选择流程
```mermaid
graph TD 
    B5["依据构建流程中确定的FilterType<br/>确定使用的IRequestExecutingLocker及ResponseCachingContext"]
        B5 --FilterType.Resource--> B5A[依据LockMode确定IRequestExecutingLocker]
            B5A --ActionSingle--> B5A1["从DI中获取IActionSingleResourceExecutingLocker"] --> B6A
            B5A --CacheKeySingle--> B5A2["从DI中获取ICacheKeySingleResourceExecutingLocker"] --> B6A
            B5A --其它--> B5A3["不使用IRequestExecutingLocker"] --> B6A
        B5 --FilterType.Action--> B5B[依据LockMode确定IRequestExecutingLocker]
            B5B --ActionSingle--> B5B1["从DI中获取IActionSingleActionExecutingLocker"] --> B6B
            B5B --CacheKeySingle--> B5B2["从DI中获取ICacheKeySingleActionExecutingLocker"] --> B6B
            B5B --其它--> B5B3["不使用IRequestExecutingLocker"] --> B6B
    B6A["创建ResponseCachingContext<ResourceExecutingContext, ResponseCacheEntry>"] -->
        创建DefaultResourceCacheFilter --> BE
    B6B["创建ResponseCachingContext<ActionExecutingContext, IActionResult>"] -->
        创建DefaultActionCacheFilter --> BE
    BE["构建完成"]
```

## 2. Filter执行过程

### 2.1 ResourceFilter执行过程
```mermaid
graph TD 
    PS["OnResourceExecutionAsync"] -->
    P1["使用ICacheKeyGenerator生成缓存Key"]
        P1 --key长度大于ResponseCachingOptions.MaxCacheKeyLength--> P11["执行请求体"] --> PE
        P1 --key为空--> P12["执行请求体"] --> PE
        P1 --> P2
    P2["尝试从IResponseCache中获取ResponseCacheEntry"]
        P2 --获取到ResponseCacheEntry--> P3
        P2 --没有获取到ResponseCacheEntry--> P22["是否有IRequestExecutingLocker进行锁定"]
            P22 --有--> P221["使用IRequestExecutingLocker加锁并执行操作"]
                P221 --锁定时等待到了响应缓存--> P3
                P221 --需要执行操作--> P222
            P22 --没有--> P222["从IDumpStreamFactory获取dumpStream并替换Http响应流"] --> P2221["执行请求体"] --> P22211["从响应中获取ResponseCacheEntry"] --> P222111["使用IResponseCacheDeterminer确定是否可以缓存响应"]
                P222111 --可以缓存--> P2221111["判断响应内容长度是否小于MaxCacheableResponseLength"]
                    P2221111 --小于等于--> P22211111["使用IResponseCache缓存响应"] -->PE
                    P2221111 --大于--> PE
                P222111 --不可缓存--> PE
    P3["将缓存内容写入响应"] --> PE
    PE["缓存处理Filter流程结束"]
```

### 2.2 ActionFilter执行过程
!!!Note:
- ActionFilter为IAsyncActionFilter和IAsyncResourceFilter的结合体，即ActionFilter既实现了IAsyncActionFilter也实现了IAsyncResourceFilter
- 处理流程为IAsyncActionFilter先执行完流程，IAsyncResourceFilter再处理响应的缓存

#### 2.2.1 IAsyncActionFilter执行过程

```mermaid
graph TD
    PA["OnActionExecutionAsync"] -->
    P1["使用ICacheKeyGenerator生成缓存Key"]
        P1 --key长度大于ResponseCachingOptions.MaxCacheKeyLength--> P11["执行请求体"] --> PAE
        P1 --key为空--> P12["执行请求体"] --> PAE
        P1 --> P2
    P2["尝试从IResponseCache中获取ResponseCacheEntry"]
        P2 --获取到ResponseCacheEntry--> P3
        P2 --没有获取到ResponseCacheEntry--> P22["是否有IRequestExecutingLocker进行锁定"]
            P22 --有--> P221["使用IRequestExecutingLocker加锁并执行操作"]
                P221 --锁定时等待到了ActionResult--> P2211["设置Context的Result为等待到的Result"] --> PAE
                P221 --需要执行操作--> P222
            P22 --没有--> P222["从IDumpStreamFactory获取dumpStream并替换Http响应流"] --> P2221["执行请求体"] --> P22211["使用IResponseCacheDeterminer确定是否可以缓存响应"] 
                P22211 --可以缓存--> P222111["将key、dumpStream、原始响应流originalBody放入HttpContext.Items"] --> PAE
                P22211 --不可缓存--> P222112["将originalBody还原到HttpContext.Response.Body并释放dumpStream"] --> PAE
    P3["将缓存内容写入响应"] --> PAE
    PAE["IAsyncActionFilter逻辑结束"]
```

#### 2.2.2 IAsyncResourceFilter执行过程

```mermaid
graph TD
    PR["OnResourceExecutionAsync"] --"等待请求后续逻辑（即IAsyncActionFilter等）完成后"-->
    PR1["从HttpContext.Items中获取key、dumpStream、原始响应流originalBody"]
        PR1 --获取到数据--> PR2
        PR1 --没有获取到数据--> PRE
    PR2["将dumpStream内容还原到originalBody并从dumpStream中获取响应内容"] -->
    PR3["判断响应内容长度是否小于MaxCacheableResponseLength"]
        PR3 --小于等于--> PR31["从响应和响应内容生成ResponseCacheEntry"] --> PR311["使用IResponseCache缓存响应"] --> PRPE
        PR3 --大于--> PRPE
    PRPE["将originalBody还原到HttpContext.Response.Body并释放dumpStream"] -->
    PRE["IAsyncResourceFilter逻辑结束"] -->
    PE["缓存处理Filter流程结束"]
```
