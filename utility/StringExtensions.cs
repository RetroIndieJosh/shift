namespace shift
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string str, string oldStr, string newStr)
        {
            var start = str.IndexOf(oldStr);
            if (start < 0)
                return str;
            return $"{str[..start]}{newStr}{str[(start + oldStr.Length)..]}";
        }
    }
}
