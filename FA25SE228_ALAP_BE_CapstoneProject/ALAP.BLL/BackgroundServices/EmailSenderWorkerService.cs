using ALAP.BLL.Interface;
using ALAP.BLL.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.BackgroundServices
{
    /// <summary>
    /// Background service xử lý gửi email từ hàng đợi
    /// </summary>
    public class EmailSenderWorkerService : BackgroundService
    {
        private readonly IBackgroundEmailQueue _emailQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailSenderWorkerService> _logger;
        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxConcurrency;
        private readonly int[] _retryDelaysSeconds;

        public EmailSenderWorkerService(
           IBackgroundEmailQueue emailQueue,
           IServiceScopeFactory scopeFactory,
           ILogger<EmailSenderWorkerService> logger,
           IOptions<EmailWorkerOptions> options)
        {
            _emailQueue = emailQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _maxConcurrency = options.Value.MaxConcurrency;
            _semaphore = new SemaphoreSlim(_maxConcurrency);
            _retryDelaysSeconds = options.Value.RetryDelaysSeconds;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailSenderWorkerService đã khởi động. MaxConcurrency: {MaxConcurrency}",
                _maxConcurrency);

            var tasks = new List<Task>();

            try
            {
                await foreach (var emailMessage in _emailQueue.Reader.ReadAllAsync(stoppingToken))
                {
                    // Đợi slot available
                    await _semaphore.WaitAsync(stoppingToken);

                    // Tạo task xử lý email
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await SendEmailWithRetryAsync(emailMessage, stoppingToken);
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }, stoppingToken);

                    tasks.Add(task);

                    // Dọn dẹp các task đã hoàn thành
                    tasks.RemoveAll(t => t.IsCompleted);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("EmailSenderWorkerService đang dừng lại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong EmailSenderWorkerService");
            }

            // Đợi tất cả task đang chạy hoàn thành
            if (tasks.Any())
            {
                _logger.LogInformation("Đang đợi {Count} email còn lại được gửi...", tasks.Count);
                await Task.WhenAll(tasks);
            }

            _logger.LogInformation("EmailSenderWorkerService đã dừng");
        }

        private async Task SendEmailWithRetryAsync(EmailMessage emailMessage, CancellationToken cancellationToken)
        {
            for (int attempt = 0; attempt <= _retryDelaysSeconds.Length; attempt++)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailBizLogic>();

                        var success = await emailSender.SendEmailAsync(
                            emailMessage.To,
                            emailMessage.Subject,
                            emailMessage.HtmlBody,
                            isHtml: true);

                        if (success)
                        {
                            _logger.LogDebug("Đã gửi email thành công đến {To}: {Subject}",
                                emailMessage.To, emailMessage.Subject);
                            return;
                        }
                        else
                        {
                            _logger.LogWarning("Gửi email thất bại (lần {Attempt}) đến {To}: {Subject}",
                                attempt + 1, emailMessage.To, emailMessage.Subject);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi gửi email (lần {Attempt}) đến {To}: {Subject}",
                        attempt + 1, emailMessage.To, emailMessage.Subject);
                }

                if (attempt < _retryDelaysSeconds.Length)
                {
                    var delaySeconds = _retryDelaysSeconds[attempt];
                    _logger.LogDebug("Đợi {Delay}s trước khi thử lại gửi email đến {To}",
                        delaySeconds, emailMessage.To);

                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
                }
            }

            _logger.LogError("Không thể gửi email đến {To} sau {Attempts} lần thử: {Subject}",
                emailMessage.To, _retryDelaysSeconds.Length + 1, emailMessage.Subject);
        }


        public override void Dispose()
        {
            _semaphore?.Dispose();
            base.Dispose();
        }
    }

    /// <summary>
    /// Cấu hình cho EmailSenderWorkerService
    /// </summary>
    public class EmailWorkerOptions
    {
        public const string SectionName = "EmailWorker";

        /// <summary>
        /// Số lượng email gửi đồng thời tối đa
        /// </summary>
        public int MaxConcurrency { get; set; } = 10;

        /// <summary>
        /// Thời gian chờ giữa các lần retry (giây)
        /// </summary>
        public int[] RetryDelaysSeconds { get; set; } = new[] { 2, 5, 15 };
    }
}

