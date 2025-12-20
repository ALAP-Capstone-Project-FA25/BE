using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IEventBizLogic
    {
        Task<bool> CreateUpdateEvent(CreateUpdateEventDto dto);
        Task<EventModel> GetEventById(long id);
        Task<PagedResult<EventModel>> GetListEventsByPaging(PagingModel pagingModel);
        Task<bool> DeleteEvent(long id);
        Task<bool> SendCommissionToSpeaker(long eventId, string paymentProofImageUrl = "");
        Task<CancelEventResultDto> CancelEvent(long eventId);
    }
}

