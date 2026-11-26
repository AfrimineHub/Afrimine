namespace Afrimine.Shared.Helpers
{
    public class GetEmailTemplate
    {
        public static string GetConfirmEmailTemplate(string otp)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ConfirmEmail.html");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            
            body = File.ReadAllText(filePath);

            var msgBody = body.Replace("{otp}", otp).Replace("{year}", DateTime.Now.Year.ToString());

            return msgBody;


        }

        public static string GetWithdrawalOtpTemplate(string otp, string amount)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "WithdrawalOtp.html");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            return File.ReadAllText(filePath)
                .Replace("{otp}", otp)
                .Replace("{amount}", System.Net.WebUtility.HtmlEncode(amount))
                .Replace("{year}", DateTime.Now.Year.ToString());
        }

        public static string GetResetPasswordEmailTemplate(string otp)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ResetPassword.html");
            var filePath = Path.Combine (Directory.GetCurrentDirectory(), folderName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            
            body = File.ReadAllText(filePath);

            var msgBody = body.Replace("{otp}", otp).Replace("{year}", DateTime.Now.Year.ToString());

            return msgBody;
        }
    }
}
