using System.ComponentModel;

namespace BarberShop.Utility
{
    public class Enuns
    {
        public enum Role
        {
            [Description("Administrador")]
            Admin = 0,
            [Description("Barbeiro")]
            Barber = 1,
            [Description("Client")]
            Cliente = 2
        }

        public static string GetEnumDescription(System.Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo != null)
            {
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }

            // Se não houver um atributo Description, retorna o nome do enum como padrão
            return value.ToString();
        }
    }
}
