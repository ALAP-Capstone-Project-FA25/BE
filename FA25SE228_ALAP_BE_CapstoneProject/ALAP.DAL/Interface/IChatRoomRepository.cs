using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IChatRoomRepository
    {
        Task<bool> Create(ChatRoomModel model);
        Task<bool> Update(ChatRoomModel model);
        Task<ChatRoomModel?> GetById(long id);
        Task<PagedResult<ChatRoomModel>> GetListByPaging(PagingModel pagingModel);
        Task<bool> Delete(long id);
        Task<List<ChatRoomModel>> GetByMentorId(long mentorId);
    }
}

