using MimeKit;
using MailKit.Net.Smtp;


namespace Notifier.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта Learn Platform", "yayaya556@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.yandex.ru", 25, false);
                    await client.AuthenticateAsync("yayaya556@yandex.ru", "dmvbmawplebwiibl"); // send our email from point "yayaya556@yandex.ru", with special password "dmvbmawplebwiibl"
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);

                } catch (Exception ex)
                { 
                    Console.WriteLine(ex.ToString());
                }
                
            }
        }
    }
}
