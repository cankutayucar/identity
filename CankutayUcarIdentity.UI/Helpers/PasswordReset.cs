using System.Net;
using System.Net.Mail;

namespace CankutayUcarIdentity.UI.Helpers
{
    public static class PasswordReset
    {
        public static void PasswordResetSendEmail(string? link)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("");
            mail.To.Add("");
            mail.Subject = $"www.cankutayucar.com::şifre sıfırlama";
            mail.Body = "<h2>Şifrenizi  yenilemek için lütfen aşağıdaki linke tıklayınız</h2><hr/>";
            mail.Body += $"<a href='{link}>şifre yenileme linki</a>'";
            mail.IsBodyHtml = true;


            SmtpClient smtp = new SmtpClient("");
            smtp.Host = "";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("", "");
            smtp.Send(mail);
        }
    }
}
