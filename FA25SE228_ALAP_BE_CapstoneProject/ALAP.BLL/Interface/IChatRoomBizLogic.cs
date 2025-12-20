using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IChatRoomBizLogic
    {
        Task<bool> CreateUpdateChatRoom(CreateUpdateChatRoomDto dto);
        Task<ChatRoomModel> GetChatRoomById(long id);
        Task<PagedResult<ChatRoomModel>> GetListChatRoomsByPaging(PagingModel pagingModel);
        Task<bool> DeleteChatRoom(long id);

        Task<ChatRoomModel> GetChatRoomByCourseId(long courseId, long userId);

        Task<List<ChatRoomModel>> GetListChatRoomByMentorId(long mentorId);

    }
}

