namespace Afrimine.Shared
{
    public static class StringExtensions
    {
        public static string CapitalizeWords(this string input)
        {
            return string.Join(" ",
                input.Split(' ')
                     .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }
    }
}
