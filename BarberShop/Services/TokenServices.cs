using BarberShop.Models;
using BarberShop.Utility;
using BarberShop.Utility.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BarberShop.Services
{
    public class TokenServices : IToken
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public TokenServices(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        public bool GenerateToken(Person person)
        {
            try
            {
                TokenConfig tokenConfig = new();
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenConfig.Key = _configuration.GetValue<string>("TokenConfig:Key");

                var key = Encoding.ASCII.GetBytes(tokenConfig.Key);
                string role = Enuns.GetEnumDescription(person.Role);

                //Token vai expirar a cada 3h, será usado para definir se o usuário está logado
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, person.Id.ToString()),
                        new Claim(ClaimTypes.Role, role),
                        new Claim("Chave", "b5d8a1f7")
                    }),

                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                string tokenTest = tokenHandler.WriteToken(token);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
