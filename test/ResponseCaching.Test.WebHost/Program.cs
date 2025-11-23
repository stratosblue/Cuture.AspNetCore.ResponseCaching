using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ResponseCaching.Test.WebHost.Test;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
});

var services = builder.Services;

services.AddResponseCaching();

var redisConfigureString = builder.Configuration.GetValue<string>("ResponseCache_Test_Redis");
services.AddCaching(builder.Configuration.GetSection("Caching:ResponseCaching"))
        //.UseRedisResponseCache(redisConfigureString, Configuration.GetSection("Caching:ResponseCaching:CacheKeyPrefix").Value)
        .AddDiagnosticDebugLogger();

//HACK 清理缓存，避免影响测试 -- 连接字符串需要添加 allowAdmin=true
//{
//    var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigureString);
//    connectionMultiplexer.GetServer(redisConfigureString.Split(',')[0]).FlushAllDatabases();
//}

services.AddControllers();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("123456789123456789_123456789123456789")),
            ValidIssuer = "Issuer",
            ValidAudience = "Audience",
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

services.AddTransient<TestCustomCacheKeyGenerator>();
services.AddSingleton<TestCustomModelKeyParser>();

#if NET9_0_OR_GREATER
services.AddOpenApi();
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

#if NET9_0_OR_GREATER
    app.MapOpenApi();
    app.MapSwaggerUI();
#endif
}

app.EnableResponseCachingDiagnosticLogger();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }
