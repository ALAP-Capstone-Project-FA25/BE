using ALAP.BLL.Interface;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using ALAP.Entity.Models.Enums;

namespace ALAP.BLL.Implement
{
    public class EmailBizLogic : IEmailBizLogic
    {
        private readonly SmtpSettings _smtpSettings;
        public EmailBizLogic(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("", _smtpSettings.User));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;
                email.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

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
    }
}
