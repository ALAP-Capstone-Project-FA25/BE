using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IChatRoomMessageBizLogic
    {
        Task<bool> CreateUpdateChatRoomMessage(CreateUpdateChatRoomMessageDto dto);
        Task<ChatRoomMessageModel> GetChatRoomMessageById(long id);
        Task<PagedResult<ChatRoomMessageModel>> GetListChatRoomMessagesByPaging(PagingModel pagingModel);
        Task<List<ChatRoomMessageModel>> GetMessagesByChatRoomId(long chatRoomId);
        Task<bool> DeleteChatRoomMessage(long id);
    }
}

