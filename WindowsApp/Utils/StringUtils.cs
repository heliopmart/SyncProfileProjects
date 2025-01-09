using System.Text.RegularExpressions;

namespace WindowsApp.Utils
{
    public static class StringUtils
    {
         public static string SanitizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Converte para minúsculas
            string sanitized = input.ToLower();

            // Remove caracteres especiais
            sanitized = Regex.Replace(sanitized, @"[^\w\s]", ""); 

            // Substitui espaços por "_"
            sanitized = Regex.Replace(sanitized, @"\s+", "_"); 

            return sanitized;
        }
    }
}
