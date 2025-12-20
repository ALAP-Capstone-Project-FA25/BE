using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IMajorBizLogic
    {
        Task<bool> CreateUpdateMajor(CreateUpdateMajorDto dto);
        Task<MajorModel> GetMajorById(long id);
        Task<PagedResult<MajorModel>> GetListMajorsByPaging(PagingModel pagingModel);
        Task<bool> DeleteMajor(long id);
        Task<bool> UpdateUserMajor(long userId, long majorId);
    }
}

