using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IChatRoomMessageRepository
    {
        Task<bool> Create(ChatRoomMessageModel model);
        Task<bool> Update(ChatRoomMessageModel model);
        Task<ChatRoomMessageModel?> GetById(long id);
        Task<PagedResult<ChatRoomMessageModel>> GetListByPaging(PagingModel pagingModel);
        Task<List<ChatRoomMessageModel>> GetByChatRoomId(long chatRoomId);
        Task<bool> Delete(long id);
    }
}

