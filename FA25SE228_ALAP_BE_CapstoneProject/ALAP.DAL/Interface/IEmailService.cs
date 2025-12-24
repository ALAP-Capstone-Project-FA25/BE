using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string templateFileName, Dictionary<string, string> placeholders);
        Task SendWorker(string to, string subject, string templateFileName, Dictionary<string, string> placeholders);
    }
}
