using System.Text.Json.Serialization;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using UGB.MVC.Interfaces;

namespace UGB.MVC.Helper
{
    public class EmailService : IEmailService
    {
        private readonly SettingsBase mailSettings;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<EmailService> logger;

        public EmailService(
            IOptions<SettingsBase> options,
            IWebHostEnvironment environment,
            ILogger<EmailService> logger)
        {
            mailSettings = options.Value;
            this.environment = environment;
            this.logger = logger;
        }

        public async Task SendMail(Email emailData)
        {
            if(string.IsNullOrWhiteSpace(emailData.To))
                throw new HttpRequestException("El destinatario es requerido.");

            if(string.IsNullOrWhiteSpace(emailData.Subject))
                throw new HttpRequestException("El asunto es requerido.");

            if(string.IsNullOrWhiteSpace(emailData.Body))
                throw new HttpRequestException("El cuerpo del correo es requerido.");

            string senderAddress = string.IsNullOrWhiteSpace(mailSettings.Mail)
                ? "no-reply@localhost"
                : mailSettings.Mail;

            string displayName = string.IsNullOrWhiteSpace(mailSettings.DisplayName)
                ? "UGB MVC"
                : mailSettings.DisplayName;

            var message = new MimeMessage
            {
                Sender = new MailboxAddress(displayName, senderAddress),
                Subject = emailData.Subject
            };

            message.From.Add(new MailboxAddress(displayName, senderAddress));
            message.To.Add(MailboxAddress.Parse(emailData.To));

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailData.Body
            };

            foreach(MimePart attachment in emailData.Attachments)
            {
                bodyBuilder.Attachments.Add(
                    attachment.FileName!,
                    attachment.Content!.Stream!,
                    attachment.ContentType);
            }

            message.Body = bodyBuilder.ToMessageBody();

            if(mailSettings.UsePickupDirectory)
            {
                string pickupPath = Path.Combine(environment.ContentRootPath, mailSettings.PickupDirectory);
                Directory.CreateDirectory(pickupPath);

                string fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}.eml";
                string filePath = Path.Combine(pickupPath, fileName);

                await using var stream = File.Create(filePath);
                await message.WriteToAsync(stream);
                logger.LogInformation("Correo generado en {FilePath}", filePath);
                return;
            }

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(mailSettings.SMTP, mailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    public class Email
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

        [JsonIgnore]
        public IEnumerable<MimePart> Attachments { get; set; } = new List<MimePart>();
    }
}
