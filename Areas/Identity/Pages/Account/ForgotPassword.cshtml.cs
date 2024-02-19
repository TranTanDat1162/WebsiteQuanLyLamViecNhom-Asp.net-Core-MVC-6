// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using WebsiteQuanLyLamViecNhom.HelperClasses;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<BaseApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<BaseApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null && user.EmailConfirmed == true)
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var linkHref = "<a href = '" + Url.Action("ResetPassword", "Account",
                        new { UserId = user.Id, code = token }, protocol: Request.Scheme) + "'>Reset Password</a>";
                    string subject = "Đổi mật khẩu"; // Đổi subject lại
                    string body = "<b>Đường dẫn đổi mật khẩu</b><br/>" + linkHref; // Đổi body lại
                    SendEmailAsync(Input.Email, subject, body);
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
                else
                {
                    TempData["ErrorMessage"] = "Email không tồn tại hoặc chưa được xác nhận.";
                    return Page();
                }
            }

            return Page();
        }


        private async Task<bool> SendEmailAsync(string toEmail, string subject, string emailBody)
        {
            try
            {
                // Cần xác thực 2 step verification
                //string senderEmail = "email@gmail.com"; // Đổi gmail riêng nckh2324@gmail.com của mình
                //string senderPassword = "password"; // Tương tự
                //SmtpClient client= new SmtpClient ("smtp.gmail.com", 587);
                //client.EnableSsl = true;
                //client.Timeout = 100000;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.UseDefaultCredentials= false;
                //client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                //MailMessage mailMessage= new MailMessage(senderEmail, toEmail, subject,
                //    emailBody);
                //mailMessage.IsBodyHtml = true;
                //mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                //client.Send(mailMessage);

                await _emailSender.SendEmailAsync(toEmail, subject, emailBody);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
