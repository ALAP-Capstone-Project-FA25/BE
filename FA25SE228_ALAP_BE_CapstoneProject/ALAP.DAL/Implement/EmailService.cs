using ALAP.DAL.Interface;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using ALAP.Entity.Models.Enums;


namespace ALAP.DAL.Implement
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string templateFileName, Dictionary<string, string> placeholders)
        {
            try
            {
                string templatePath = GetTemplatePath(templateFileName);
                if (!File.Exists(templatePath))
                {
                    Console.WriteLine($"[Email Error] Không tìm thấy template: {templatePath}");
                    return false;
                }

                string body = LoadEmailTemplate(templatePath, placeholders);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("", _smtpSettings.User));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        _smtpSettings.Host,
                        _smtpSettings.Port,
                        _smtpSettings.Secure ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Pass);
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] {ex.Message}");
                return false;
            }
        }

        private string LoadEmailTemplate(string templatePath, Dictionary<string, string> replacements)
        {
            string body = File.ReadAllText(templatePath);

            foreach (var item in replacements)
            {
                body = body.Replace($"{{{{{item.Key}}}}}", item.Value);
            }

            return body;
        }


        private string GetTemplatePath(string templateFileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var templatePath = Path.Combine(basePath, "Templates", templateFileName);

            return templatePath;
        }

        public Task SendWorker(string to, string subject, string templateFileName, Dictionary<string, string> placeholders)
        {
            try
            {
                _ = Task.Run(async () =>
                {
                    await SendEmailAsync(to, subject, templateFileName, placeholders);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error SendMail] {0} {1}", ex.Message, ex.StackTrace);

            }
            return Task.CompletedTask;


        }
    }
}
