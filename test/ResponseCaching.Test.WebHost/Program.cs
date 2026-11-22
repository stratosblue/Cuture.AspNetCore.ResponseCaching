using System.Text;
using Cuture.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ResponseCaching.Test.WebHost;
using ResponseCaching.Test.WebHost.Test;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
});

var services = builder.Services;

services.AddResponseCaching();

var useRedis = args.Contains("--use-redis");

GarnetServerDescriptor? garnetServerDescriptor = null;
if (useRedis)
{
    garnetServerDescriptor = GarnetServerUtil.Create();
    services.AddSingleton(garnetServerDescriptor);
}

var cachingServiceBuilder = services.AddCaching(builder.Configuration.GetSection("Caching:ResponseCaching"))
                                    .AddDiagnosticDebugLogger();

if (useRedis)
{
    services.PostConfigure<ResponseCachingOptions>(options => options.DefaultCacheStoreLocation = CacheStoreLocation.Distributed);
    cachingServiceBuilder.UseRedisResponseCache(garnetServerDescriptor!.RedisConfigureString, builder.Configuration.GetSection("Caching:ResponseCaching:CacheKeyPrefix").Value);
}

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

#if DEBUG
services.AddOpenApi();
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

#if DEBUG
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
