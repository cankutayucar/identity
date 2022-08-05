using System.Net;
using System.Net.Mail;

namespace CankutayUcarIdentity.UI.Helpers
{
    public static class EmailConfirmation
    {
        public static void EmailConfirmSendEmail(string? link, string email)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("");
            mail.To.Add(email);
            mail.Subject = $"www.cankutayucar.com::E-mail Doğrulama";
            mail.Body = "<h2>E-mail adresinizi için lütfen aşağıdaki linke tıklayınız</h2><hr/>";
            mail.Body += $"<a href='{link}>E-mail doğrulama linki</a>'";
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
