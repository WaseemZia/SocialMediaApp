namespace API.DTOs
{
    public class ForgotPasswordDto
    {
        public string email { get; set; } = "";
    }
    public class ResetPasswordDto
    {
        public string email { get; set; } = "";
        public string ResetCode { get; set; } = "";

        public string NewPassword { get; set; } = "";

    }
}
