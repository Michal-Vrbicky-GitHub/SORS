using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SORS.Data.Models;
using System;
using System.Threading.Tasks;

namespace SORS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Generování tokenu
                    var token = GenerateRandomToken();
                    await _userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", token);

                    // Přihlášení uživatele po registraci
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(new { Message = "User registered successfully" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GetAuthenticationTokenAsync(user, "Default", "AccessToken");
                    if (string.IsNullOrEmpty(token))
                    {
                        return BadRequest(new { Message = "User does not have a valid token." });
                    }

                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return Ok(new { Message = "Login successful" });
                    }
                    return BadRequest(new { Message = "Invalid login attempt." });
                }
                return BadRequest(new { Message = "Invalid login attempt." });
            }

            return BadRequest(ModelState);
        }

        private string GenerateRandomToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }
    }
}
