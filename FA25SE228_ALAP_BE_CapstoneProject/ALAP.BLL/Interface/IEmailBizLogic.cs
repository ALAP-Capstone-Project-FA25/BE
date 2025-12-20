using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IEmailBizLogic
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    }
}
