using System;

namespace LunchRoulette.Utils.StringHelpers
{
    public static class StringMethods
    {
        public static bool EqualsIgnoreCase(this string s1, string s2)
        {
            return s2 != null && s1.ToLowerInvariant() == s2.ToLowerInvariant();
        }

        public static string ToTitleCase(this string s)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }
    }
}
