using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using MimeKit.Text;

namespace PustokApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration configuration;
        public void Send(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("eliyevaleyla232323@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Reset password";
            
                email.Body = new TextPart(TextFormat.Html) { Text = body };
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("eliyevaleyla232323@gmail.com", "hotj fmrl qhyp onnq");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
