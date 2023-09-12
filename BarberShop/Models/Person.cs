using static BarberShop.Utility.Enuns;

namespace BarberShop.Models
{
    public class Person:BaseModel
    {
        public Role Role { get; set; }

        public string FullName { get; set; }

        public string Mail { get; set; }

        public string UserName { get; set; }

        public string Pwd { get; set; }
    }
}
