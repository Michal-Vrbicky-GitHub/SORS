using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORS.Data;
using Microsoft.EntityFrameworkCore;
using SORS.Data.Models;

namespace SORS.Pages
{
    public class TokenManagementModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TokenManagementModel> _logger;

        public TokenManagementModel(UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<TokenManagementModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public List<UserTokenViewModel> UserTokens { get; set; }

        public async Task OnGetAsync()
        {/*
            UserTokens = new List<UserTokenViewModel>();

            var users = _userManager.Users;
            foreach (var user in users)
            {
                var token = await _userManager.GetAuthenticationTokenAsync(user, "Default", "AccessToken");

                UserTokens.Add(new UserTokenViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    Value = token,
                    TokenExpiry = expiryDate
                });
            }*/
            UserTokens = new List<UserTokenViewModel>();

            var tokens = await _context.UserTokens.ToListAsync();
            foreach (var token in tokens)
            {
                var user = await _userManager.FindByIdAsync(token.UserId);
                if (user != null)
                {
                    UserTokens.Add(new UserTokenViewModel
                    {
                        UserId = user.Id,
                        UserEmail = user.Email,
                        Value = token.Value,
                        TokenExpiry = token.ExpiryDate
                    });
                }
            }
        }

        public async Task<IActionResult> OnPostGenerateTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var existingToken = await _context.UserTokens
                    .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.LoginProvider == "Default" && ut.Name == "AccessToken");

                var token = GenerateRandomToken();
                if (existingToken != null)
                {
                    existingToken.Value = token;
                    existingToken.ExpiryDate = DateTime.UtcNow.AddMonths(1);
                    _context.UserTokens.Update(existingToken);
                }
                else
                {
                    var userToken = new ApplicationUserToken
                    {
                        UserId = user.Id,
                        LoginProvider = "Default",
                        Name = "AccessToken",
                        Value = token,
                        ExpiryDate = DateTime.UtcNow.AddMonths(1)
                    };
                    _context.UserTokens.Add(userToken);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
        /*
        public async Task<IActionResult> OnPostDeleteTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "AccessToken");
            }

            return RedirectToPage();
        }*//*
        public async Task<IActionResult> OnPostDeleteTokenAsync(string userId)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.LoginProvider == "Default" && ut.Name == "AccessToken");

            if (userToken != null)
            {
                _context.UserTokens.Remove(userToken);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }*/
        public async Task<IActionResult> OnPostDeleteTokenAsync(string userId)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.LoginProvider == "Default" && ut.Name == "AccessToken");

            if (userToken != null)
            {
                userToken.Value = string.Empty;
                userToken.ExpiryDate = null;

                _context.UserTokens.Update(userToken);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        /*
        public async Task<IActionResult> OnPostEditTokenAsync([FromBody] EditTokenModel model)
        {/*
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                await _userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", model.Token);
            }

            return RedirectToPage();*//*
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == model.UserId && ut.LoginProvider == "Default" && ut.Name == "AccessToken");

            if (userToken != null)
            {
                userToken.Value = model.Token;
                userToken.ExpiryDate = model.TokenExpiry;

                _context.UserTokens.Update(userToken);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }*/
        public async Task<IActionResult> OnPostEditTokenAsync([FromBody] EditTokenModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("EditTokenModel is null");
                return BadRequest("Model is null");
            }
            _logger.LogInformation("Editing token for user: {UserId}", model.UserId);
            _logger.LogInformation("New token value: {Token}", model.Token);
            _logger.LogInformation("New token expiry: {TokenExpiry}", model.TokenExpiry);

            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == model.UserId && ut.LoginProvider == "Default" && ut.Name == "AccessToken");

            if (userToken != null)
            {
                userToken.Value = model.Token;
                userToken.ExpiryDate = model.TokenExpiry;

                _context.UserTokens.Update(userToken);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
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

        public class EditTokenModel
        {
            public string UserId { get; set; }
            public string Token { get; set; }
            public DateTime? TokenExpiry { get; set; }
        }
    }

    public class UserTokenViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Value { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
