using FluentAssertions.Equivalency;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebsiteQuanLyLamViecNhom.Areas.Identity.Pages.Account;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APILoginController : ControllerBase
    {
        private readonly SignInManager<BaseApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<BaseApplicationUser> _userManager;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public APILoginController(SignInManager<BaseApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<BaseApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; } 
            public bool RememberMe { get; set; }

        }
        [HttpPost]
        public async Task<ActionResult<object>> LoginAuthenticate([FromBody] InputModel Input)
        {
            _logger.LogInformation("Username: {username}, Password: {password}", Input.Username, Input.Password);

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(Input.Username);
                    var roles = await _userManager.GetRolesAsync(user);
                    if (user.IsLocked)
                    {
                        if (roles.Contains("Student"))
                        {
                            _logger.LogInformation("APIStudent logged in.");
                            return new { status = 200, message = "Student's account verified", Data = (Student)user };
                        }
                        else if (roles.Contains("Teacher"))
                        {
                            _logger.LogInformation("APITeacher logged in.");
                            return new { status = 200, message = "Teacher's account verified", Data = (Teacher)user };
                        }
                        else if (roles.Contains("Admin"))
                        {
                            _logger.LogInformation("APIAdmin logged in.");
                            return new { status = 200, message = "Admin's account verified", Data = user };
                        }
                    }
                    else
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                }
                else
                {
                    if (ModelState.ContainsKey("Username") && ModelState["Username"].Errors.Count > 0)
                    {
                        return new { status = 401, message = "Invalid username" };
                    }
                    else if (ModelState.ContainsKey("Password") && ModelState["Password"].Errors.Count > 0)
                    {
                        return new { status = 401, message = "Invalid password" };
                    }
                    else
                    {
                        return new { status = 401, message = "Username or password is incorrect" };
                    }
                }
            }
            return new { status = 400, message = "Model is Invalid" };
            
        }
    }
}
