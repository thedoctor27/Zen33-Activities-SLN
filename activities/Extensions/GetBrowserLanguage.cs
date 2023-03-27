using Microsoft.Extensions.Primitives;

namespace activities.Extensions
{
    public class GetBrowserLanguage
    {
        public static string GetLanguageFromHeader(HttpContext? context)
        {
            if (context == null)
            {
                return "EN";
            }
            try
            {
                string lang = context.Request.Headers["Accept-Language"].FirstOrDefault().ToUpper();
                return lang.Split(',')[0];
            }
            catch
            {
                return "EN";
            }
        }
    }
}
