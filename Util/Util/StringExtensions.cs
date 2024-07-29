namespace Evil.Util
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return char.ToUpper(str[0]) + str[1..];
        }
    }
}