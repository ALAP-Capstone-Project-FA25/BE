using ALAP.Entity.DTO.Response;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface ISearchBizLogic
    {
        Task<SearchResultDto> SearchAll(string keyword, int limit = 10);
    }
}
