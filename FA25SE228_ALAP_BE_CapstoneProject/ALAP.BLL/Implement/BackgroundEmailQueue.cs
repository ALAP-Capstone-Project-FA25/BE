using ALAP.BLL.Interface;
using ALAP.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class BackgroundEmailQueue : IBackgroundEmailQueue
    {
        private readonly Channel<EmailMessage> _channel;

        public BackgroundEmailQueue()
        {
            // Tạo unbounded channel để không giới hạn số lượng email trong queue
            // Trong production có thể dùng bounded channel với capacity phù hợp
            var options = new UnboundedChannelOptions
            {
                SingleReader = false, // Cho phép nhiều worker đọc
                SingleWriter = false  // Cho phép nhiều nguồn ghi
            };
            _channel = Channel.CreateUnbounded<EmailMessage>(options);
        }

        public void Enqueue(EmailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!_channel.Writer.TryWrite(message))
            {
                throw new InvalidOperationException("Không thể thêm email vào hàng đợi");
            }
        }

        public ChannelReader<EmailMessage> Reader => _channel.Reader;
    }
}

