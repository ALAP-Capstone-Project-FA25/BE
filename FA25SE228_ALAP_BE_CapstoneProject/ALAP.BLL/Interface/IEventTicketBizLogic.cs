using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IEventTicketBizLogic
    {
        Task<bool> CreateUpdateEventTicket(CreateUpdateEventTicketDto dto);
        Task<EventTicketModel> GetEventTicketById(long id);
        Task<PagedResult<EventTicketModel>> GetListEventTicketsByPaging(PagingModel pagingModel, long eventId);
        Task<bool> DeleteEventTicket(long id);
        Task<string> BuyTicket(long eventId, long userId);
        Task<List<EventTicketModel>> GetMyTickets(long userId);
        Task<PagedResult<EventTicketModel>> GetRefundList(RefundFilterModel filter);
        Task<bool> UpdateRefundStatus(UpdateRefundDto dto);
        Task<RefundStatisticsDto> GetRefundStatistics(long eventId);
        Task<RefundStatisticsDto> GetRefundStatisticsOverall();
        Task<UserTicketStatusDto> CheckUserTicket(long eventId, long userId);
    }
}

