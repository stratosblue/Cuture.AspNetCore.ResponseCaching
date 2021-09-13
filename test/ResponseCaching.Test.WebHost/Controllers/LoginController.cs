﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using IdentityModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ResponseCaching.Test.WebHost.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        #region Private 字段

        private readonly ILogger<LoginController> _logger;

        #endregion Private 字段

        #region Public 构造函数

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        #endregion Public 构造函数

        #region Public 方法

        [HttpGet]
        public async Task CookieAsync([FromQuery] string uid)
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(JwtClaimTypes.Id,uid),
                new Claim(JwtClaimTypes.SessionId,new string(uid.Reverse().ToArray()))
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }

        [HttpGet]
        public string Jwt([FromQuery] string uid)
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Id,uid),
                new Claim(JwtClaimTypes.SessionId,new string(uid.Reverse().ToArray()))
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("123456789123456789"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken("Issuer", "Audience", claims, expires: DateTime.Now.AddMinutes(600), signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }

        #endregion Public 方法
    }
}