using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IPaymentBizLogic
    {
        Task<PaymentModel> GetPaymentByOrderCode(long orderCode);
        Task<PaymentModel> UpdatePaymentByOrderCode(long orderCode, PaymentStatus status);
        Task<PagedResult<PaymentModel>> GetPaymentByPaging(PagingModel pagingModel, PaymentStatus? status = null);
        Task<PaymentModel> UpdatePaymentStatus(long id, PaymentStatus status);
        Task<object> GetPaymentStatistics();
    }
}
