using System.Text;
using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [ApiController]
    public class AccountController : BaseApiController
    {

        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender<User> _emailSender;
        private readonly IConfiguration config;
        public AccountController(SignInManager<User> signInManager, IEmailSender<User> emailSender,
            IConfiguration config)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            this.config = config;
        }
        [AllowAnonymous]
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return BadRequest("UserId and code are required");

            var user = await _signInManager.UserManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _signInManager.UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Ok("Email confirmed successfully");

            return BadRequest("Email confirmation failed");
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
        {
            var user = new User
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.Email
            };
            var result = await _signInManager.UserManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await SendConfirmationEmailAsync(user, registerDto.Email);
                return Ok();
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
        }
        [AllowAnonymous]
        [HttpGet("resendConfirmEmail")]
        public async Task<ActionResult> ResendConfirmEmail(string? email, string? userId)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(userId))
            {
                return BadRequest("Email or UserId must be provided");
            }
            var user = await _signInManager.UserManager.Users.FirstOrDefaultAsync
                (x => x.Email == email || x.Id == userId);
            if (user == null || string.IsNullOrEmpty(user.Email)) return BadRequest("User not found");
            await SendConfirmationEmailAsync(user, user.Email);
            return Ok();
        }
        private async Task SendConfirmationEmailAsync(User user, string email)
        {
            var code = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var confirmEmailUrl = $"{config["ClientAppUrl"]}/confirm-email?userId={user.Id}&code={code}";


            await _emailSender.SendConfirmationLinkAsync(user, email, confirmEmailUrl);
        }

        [AllowAnonymous]
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUser()
        {
            if (User.Identity?.IsAuthenticated == false) return NoContent();

            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user == null) return NoContent();
            return Ok(
                new
                {
                    user.DisplayName,
                    user.Email,
                    user.Id,
                    user.ImageUrl
                });
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem();

            // 1) Find user by email
            var user = await _signInManager.UserManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { error = "Invalid email or password." });

            // 2) Require confirmed email (you enabled this in Program.cs)
            var requireConfirmed = _signInManager.Options.SignIn.RequireConfirmedEmail;
            if (requireConfirmed && !await _signInManager.UserManager.IsEmailConfirmedAsync(user))
            //return Forbid("EmailNotConfirmed");
            {
                // 401 with a clear reason
                return Unauthorized(new { error = "NotAllowed" });
            }

            // 3) Sign in (creates auth cookie)
            var result = await _signInManager.PasswordSignInAsync(
                userName: user.UserName!,
                password: dto.Password,
                isPersistent: dto.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {

                return Ok(1);
            }

            //if (result.IsLockedOut) return Forbid("LockedOut");
            //if (result.RequiresTwoFactor) return Forbid("RequiresTwoFactor");

            //return Unauthorized(new { error = "Invalid email or password." });
            return Problem(
            title: "Unauthorized",
            statusCode: StatusCodes.Status401Unauthorized,
            detail: "Failed",
               type: "https://httpstatuses.com/401"
);

        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto passwordDto)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _signInManager.UserManager.ChangePasswordAsync(user, passwordDto.CurrentPassword, passwordDto.NewPassword);
            if (result.Succeeded) { return Ok(); }
            return BadRequest(result.Errors.First().Description);

        }
        [AllowAnonymous]
        [HttpPost("forgotPassword")]
        public async Task<ActionResult> ForgetPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(forgotPasswordDto.email);
            if (user == null || !await _signInManager.UserManager.IsEmailConfirmedAsync(user)) return Ok();
            var code = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
            var deCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            await _emailSender.SendPasswordResetCodeAsync(user, forgotPasswordDto.email, deCode);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("resetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(resetPasswordDto.email);
            if (user == null) return Ok();
            //var code = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);

            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDto.ResetCode));
            var result = await _signInManager.UserManager.ResetPasswordAsync(user, decoded, resetPasswordDto.NewPassword);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors.First().Description);
        }

    }
}
