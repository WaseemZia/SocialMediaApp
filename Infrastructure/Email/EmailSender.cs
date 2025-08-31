
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Resend;

namespace Infrastructure.Email
{
    public class EmailSender : IEmailSender<User>
    {
        private readonly IResend _resend;
        private readonly IConfiguration config;

        public EmailSender(IResend resend,IConfiguration config)
        {
            _resend = resend;
            this.config = config;
        }
        public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            var subject = "Confirm your email address";
            var body = $@"
            <p>Hi {user.DisplayName},</p>
            <p>Please confirm your email by clicking the link below:</p>
            <p><a href=""{confirmationLink}"">Click here to verify your email</a></p>
            <p>Thanks,</p>";

            await SendMailAsync(email, subject, body);
        }

        private async Task SendMailAsync(string email, string subject, string body)
        {

            var message = new EmailMessage
            {
                From = "Whatever@resend.dev",
                Subject = subject,
                HtmlBody = body
            };
            message.To.Add(email);
            Console.WriteLine(message.HtmlBody);
            await _resend.EmailSendAsync(message);
            //await Task.CompletedTask;
        }

        public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            var subject = "Reset your password";
            var body = $@"
            <p>Hi {user.DisplayName},</p>
            <p>Please click this link to reset your password</p>
            <p><a href='{config["ClientAppUrl"]}/reset-password?email={email}&code={resetCode}'>
                Click to reset your password</a>
            </p>
            <p>If you did not request this, you can ignore this email
            </p>
        ";

            await SendMailAsync(email, subject, body);
        }

        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}

