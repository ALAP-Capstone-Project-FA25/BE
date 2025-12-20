using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IEventTransitionService
    {
        /// <summary>
        /// Chuyển trạng thái từ IN_COMING sang IN_PROGRESS cho các sự kiện đã đến thời gian bắt đầu
        /// </summary>
        Task<int> TransitionIncomingToInProgressAsync(DateTime now, int batchSize, CancellationToken cancellationToken);

        /// <summary>
        /// Chuyển trạng thái từ IN_PROGRESS sang COMPLETED cho các sự kiện đã kết thúc
        /// </summary>
        Task<int> TransitionInProgressToCompletedAsync(DateTime now, int batchSize, CancellationToken cancellationToken);

        /// <summary>
        /// Tạo notification cho các sự kiện sắp đến hạn (1 ngày trước khi bắt đầu)
        /// </summary>
        Task<int> CreateUpcomingEventNotificationsAsync(DateTime now, int batchSize, CancellationToken cancellationToken);
    }
}

