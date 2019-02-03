using System.Text.RegularExpressions;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    public static class StringExtensions
    {
        public static string SeparateWords(this string value, string separator = " ")
        {
            return value == null ? null : Regex.Replace(value, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1" + separator);
        }
    }
}
