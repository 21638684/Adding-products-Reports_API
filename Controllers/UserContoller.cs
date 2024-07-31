using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Assignment3_Backend.Models;
using Assignment3_Backend.ViewModels;
using System.IO;

namespace Assignment3_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            var user = new AppUser { UserName = model.emailaddress, Email = model.emailaddress };
            var result = await _userManager.CreateAsync(user, model.password);
            if (result.Succeeded)
            {
                return Ok(new { message = "Registered successfully." });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.emailaddress, model.password, false, false);
            if (result.Succeeded)
            {
                return Ok(new { message = "Login successful." });
            }
            return Unauthorized(new { message = "Invalid login attempt." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful." });
        }
    }
}

