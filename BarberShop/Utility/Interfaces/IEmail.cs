using BarberShop.Models;

namespace BarberShop.Utility.Interfaces
{
    public interface IEmail
    {
        bool Send(Mail mail);
    }
}
