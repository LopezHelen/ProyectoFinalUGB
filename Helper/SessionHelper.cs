using System.Globalization;
using System.Security.Claims;

namespace UGB.MVC.Aplicaciones.Seguras.Helper
{
    public static class SessionHelper
    {
        public static dynamic GetProperty(this ClaimsPrincipal claimsPrincipal, string propertyName, Type type)
        {
            if(type == typeof(DateTime))
            {
                string value = claimsPrincipal.FindFirstValue(propertyName)!;

                if(DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }

                if(DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }

                return DateTime.Now;
            }

            return Convert.ChangeType(claimsPrincipal.FindFirstValue(propertyName), type)!;
        }

        public static dynamic GetProperty(this ClaimsPrincipal claimsPrincipal, string propertyName)
        {
            return claimsPrincipal.FindFirstValue(propertyName)!.ToString();
        }
    }
}
