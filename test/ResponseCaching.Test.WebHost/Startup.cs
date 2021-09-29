using System;
using System.Text;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

#if DEBUG
using Microsoft.OpenApi.Models;
#endif

using ResponseCaching.Test.WebHost.Test;

using StackExchange.Redis;

namespace ResponseCaching.Test.WebHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();

            var redisConfigureString = Configuration.GetValue<string>("ResponseCache_Test_Redis");
            services.AddCaching(Configuration.GetSection("Caching:ResponseCaching"))
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("123456789123456789")),
                        ValidIssuer = "Issuer",
                        ValidAudience = "Audience",
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddTransient<TestCustomCacheKeyGenerator>();
            services.AddSingleton<TestCustomModelKeyParser>();

#if DEBUG
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });
            });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

#if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI();
#endif

            app.EnableResponseCachingDiagnosticLogger();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}