namespace Serpent.IntermediateLanguageTools.Helpers
{
    using System.Globalization;

    public static class StringExtensions
    {
        public static string ToUpperFirst(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper();
            }

            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        public static string ToUpperFirst(this string text, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper(culture);
            }

            return text.Substring(0, 1).ToUpper(culture) + text.Substring(1);
        }

        public static string ToUpperFirstAfter(this string text, int startIndex)
        {
            if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length)
            {
                return text;
            }

            return text.Substring(0, startIndex) + text.Substring(startIndex, 1).ToUpper() + (text.Length <= startIndex + 1 ? string.Empty : text.Substring(startIndex + 1));
        }

        public static string ToUpperFirstAfter(this string text, int startIndex, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length)
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper(culture);
            }

            return text.Substring(0, startIndex) + text.Substring(startIndex, 1).ToUpper(culture) + (text.Length <= startIndex + 1 ? string.Empty : text.Substring(startIndex + 1));
        }

        public static string ToUpperFirstAfterFirstInstance(this string text, char firstInstance, CultureInfo culture)
        {
            var index = text.IndexOf(firstInstance);
            if (index == -1)
            {
                return text;
            }

            return text.ToUpperFirstAfter(index + 1, culture);
        }

        public static string ToUpperFirstAfterFirstInstance(this string text, char firstInstance)
        {
            var index = text.IndexOf(firstInstance);
            if (index == -1)
            {
                return text;
            }

            return text.ToUpperFirstAfter(index + 1);
        }
    }
}