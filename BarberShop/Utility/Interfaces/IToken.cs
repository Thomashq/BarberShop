using BarberShop.Models;

namespace BarberShop.Utility.Interfaces
{
    public interface IToken
    {
        bool GenerateToken(Person person);
    }
}
