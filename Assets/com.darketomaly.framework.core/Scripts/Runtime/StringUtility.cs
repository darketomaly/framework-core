namespace Framework
{
    public static class StringUtility
    {
        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + suffix;
        }

        public static string TrimEnd(this string source, string value)
        {
            if (!source.EndsWith(value))
            {
                return source;
            }

            return source.Remove(source.LastIndexOf(value));
        }

        public static string TrimStart(this string source, string value)
        {
            if (!source.StartsWith(value))
            {
                return source;
            }

            return source.Substring(value.Length);
        }
    }
}
