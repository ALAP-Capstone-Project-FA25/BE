using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFU.Common.Models
{
    public class ConfirmEmailModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
