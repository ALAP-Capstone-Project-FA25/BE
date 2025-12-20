using ALAP.BLL.Interface;
using Base.Common;
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
    /// Background service chạy định kỳ để kiểm tra và chuyển trạng thái sự kiện
    /// </summary>
    public class EventStatusSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventStatusSchedulerService> _logger;
        private readonly TimeSpan _interval;
        private readonly int _batchSize;
        private readonly int _maxLoopsPerTick;

        public EventStatusSchedulerService(
            IServiceProvider serviceProvider,
            ILogger<EventStatusSchedulerService> logger,
            IOptions<EventSchedulerOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _interval = TimeSpan.FromMinutes(options.Value.IntervalMinutes);
            _batchSize = options.Value.BatchSize;
            _maxLoopsPerTick = options.Value.MaxLoopsPerTick;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventStatusSchedulerService đã khởi động. Interval: {Interval}, BatchSize: {BatchSize}",
                _interval, _batchSize);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            using var timer = new PeriodicTimer(_interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessEventTransitionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi xử lý chuyển trạng thái sự kiện");
                }

                try
                {
                    await timer.WaitForNextTickAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("EventStatusSchedulerService đang dừng lại");
                    break;
                }
            }

            _logger.LogInformation("EventStatusSchedulerService đã dừng");
        }

        private async Task ProcessEventTransitionsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var transitionService = scope.ServiceProvider.GetRequiredService<IEventTransitionService>();

            var now = Utils.GetCurrentVNTime();
            var loops = 0;
            var totalProcessed = 0;

            // Tạo notification cho event sắp đến hạn
            await transitionService.CreateUpcomingEventNotificationsAsync(
                now, _batchSize, cancellationToken);

            // Xử lý theo batch cho đến khi không còn sự kiện cần chuyển hoặc đạt giới hạn vòng lặp
            while (loops < _maxLoopsPerTick && !cancellationToken.IsCancellationRequested)
            {
                var incomingProcessed = await transitionService.TransitionIncomingToInProgressAsync(
                    now, _batchSize, cancellationToken);

                var completedProcessed = await transitionService.TransitionInProgressToCompletedAsync(
                    now, _batchSize, cancellationToken);

                var processed = incomingProcessed + completedProcessed;
                totalProcessed += processed;

                if (processed == 0)
                {
                    break;
                }

                loops++;

                if (processed > 0 && loops < _maxLoopsPerTick)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }

            if (totalProcessed > 0)
            {
                _logger.LogInformation("Đã xử lý tổng cộng {Count} sự kiện trong {Loops} vòng lặp",
                    totalProcessed, loops);
            }
        }
    }

    /// <summary>
    /// Cấu hình cho EventStatusSchedulerService
    /// </summary>
    public class EventSchedulerOptions
    {
        public const string SectionName = "EventScheduler";

        /// <summary>
        /// Khoảng thời gian giữa các lần chạy (phút)
        /// </summary>
        public int IntervalMinutes { get; set; } = 1;

        /// <summary>
        /// Số lượng sự kiện xử lý mỗi batch
        /// </summary>
        public int BatchSize { get; set; } = 200;

        /// <summary>
        /// Số vòng lặp tối đa mỗi lần chạy 
        /// </summary>
        public int MaxLoopsPerTick { get; set; } = 10;
    }
}

