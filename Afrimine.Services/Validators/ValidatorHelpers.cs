using System.Text.RegularExpressions;

namespace Afrimine.Services.Validators
{
    public static class ValidatorHelpers
    {
        private static readonly Regex E164Regex = new Regex(@"^\+[1-9]\d{1,14}$", RegexOptions.Compiled);

        public static bool ValidE164(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            phone = phone.Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "");

            phone = Regex.Replace(phone, @"(?!^\+)\D", "");
            return E164Regex.IsMatch(phone);
        }

        public static bool PasswordMatches(string password, string confirmPassword)
        {
            if(string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return false;
            }

            return password.ToLower().Equals(confirmPassword.ToLower());
        }

        public static bool ValidFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return false;
            }

            var splits = fullName.Split(" ");
            return splits.Length > 0;
        }
    }
}
