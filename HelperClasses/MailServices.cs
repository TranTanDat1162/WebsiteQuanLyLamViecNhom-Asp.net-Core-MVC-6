using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;

namespace WebsiteQuanLyLamViecNhom.HelperClasses
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
    public class MailServices : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //phương thức đơn giản sử dụng SMTP
            //có gì thêm vô mấy cái config, t chỉ làm đơn giản nhất thôi
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                
                //Note: tương lai nên bỏ mấy cái secrets vào program.cs hoặc config cho bảo mật hơn
                //chứ giờ ng ta muốn thì lên git hub check thì thầy hết.

                //"uptd oref gpai zrjz" là mật khẩu dành riêng cho app này
                //chứ không phải là mk mail chính của t đâu
                Credentials = new NetworkCredential("trieulm20@uef.edu.vn", "uptd oref gpai zrjz")
            };
            var mailMessage = new MailMessage(from: "trieulm20@uef.edu.vn", to: email, subject, message);
            mailMessage.IsBodyHtml = true;
            //mailMessage.BodyEncoding = UTF8Encoding.UTF8;

            return client.SendMailAsync(mailMessage);


            //return client.SendMailAsync(
            //    new MailMessage(from: "trieulm20@uef.edu.vn",
            //                    to: email,
            //                    subject,
            //                    message
            //                    ));
        }
    }
}
