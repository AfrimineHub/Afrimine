namespace Afrimine.Services.Responses
{
    internal static class ResponseMessages
    {
        internal static readonly string InvalidRequest = "Invalid request.";
        internal static readonly string InvalidRegistrationRole = "You cannot register as a {0}.";
        internal static readonly string ExistingUser = "There is an existing user with this email or phone number.";
        internal static readonly string RegistrationFailed = "Registration failed. Please try again later.";
        internal static readonly string RegistrationSuccessful = "Registration successful. Please use the OTP sent to your email to verify your account.";
        internal static readonly string InvalidEmailOrPassword = "Please enter a valid email or password";
        internal static readonly string UserRecordNotFound = "User record not found";
        internal static readonly string EmailNotConfirmedOrInactive = "Email not yet confirmed or account inactive";
        internal static readonly string WrongPassword = "Please enter a correct password";
        internal static readonly string NoAssignedRole = "You can not login at the moment";
        internal static readonly string OtpNotFound = "OTP not found or expired";
        internal static readonly string OtpExpired = "Expired OTP. Please generate another one and try again";
        internal static readonly string InvalidOtp = "The OTP is invalid";
        internal static readonly string UserNotFound = "No user found";
        internal static readonly string PasswordResetFailed = "Password reset failed";
        internal static readonly string NotAuthenticated = "User not authenticated";
    }
}
