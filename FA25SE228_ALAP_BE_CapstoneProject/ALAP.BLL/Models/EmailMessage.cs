using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Models
{
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string? RecipientName { get; set; }

        public EmailMessage() { }

        public EmailMessage(string to, string subject, string htmlBody, string? recipientName = null)
        {
            To = to;
            Subject = subject;
            HtmlBody = htmlBody;
            RecipientName = recipientName;
        }
    }
}

