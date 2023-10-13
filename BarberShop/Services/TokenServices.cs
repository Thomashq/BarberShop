using BarberShop.Models;
using BarberShop.Utility;
using BarberShop.Utility.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BarberShop.Services
{
    public class TokenServices : IToken
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenServices(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public void GenerateToken(IEnumerable<Claim> claims)
        {
            try
            {
                TokenConfig tokenConfig = new();

                var tokenHandler = new JwtSecurityTokenHandler();

                tokenConfig.Key = _configuration.GetValue<string>("TokenConfig:Key");

                var key = Encoding.ASCII.GetBytes(tokenConfig.Key);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                string encryptedToken = tokenHandler.WriteToken(token);

                SaveTokenOnCookie(encryptedToken);
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC21, ex);
            }
        }

        private void SaveTokenOnCookie(string encryptedToken)
        {
            try
            {
                IHttpContextAccessor.HttpContext.Response.Cookies.Append("token_auth", encryptedToken,
                    new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(3),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None
                    });
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC23);
            }
        }
    }
}
