using ALAP.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IBackgroundEmailQueue
    {
        /// <summary>
        /// Thêm email vào hàng đợi để gửi
        /// </summary>
        void Enqueue(EmailMessage message);

        /// <summary>
        /// Reader để đọc email từ hàng đợi
        /// </summary>
        ChannelReader<EmailMessage> Reader { get; }
    }
}

