using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.Entity.Models.Enums
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Secure { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
    }
}
