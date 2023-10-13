using BarberShop.Models;
using System.Security.Claims;

namespace BarberShop.Utility.Interfaces
{
    public interface IToken
    {
        void GenerateToken(IEnumerable<Claim> claims);
    }
}
