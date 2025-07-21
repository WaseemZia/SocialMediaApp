using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    public class AccountController : BaseApiController
    {

        private readonly SignInManager<User> _signInManager;
        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
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
            if (result.Succeeded) return Ok();
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
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

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
