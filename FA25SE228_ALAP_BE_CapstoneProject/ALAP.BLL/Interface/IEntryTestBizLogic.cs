using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IEntryTestBizLogic
    {
        Task<bool> CreateUpdateEntryTest(CreateUpdateEntryTestDto dto);
        Task<bool> DeleteEntryTest(long id);
        Task<EntryTestModel> GetEntryTestById(long id);
        Task<PagedResult<EntryTestModel>> GetListEntryTestsByPaging(PagingModel pagingModel);
        Task<List<EntryTestModel>> GetAllActiveEntryTests();
        Task<EntryTestResultDto> SubmitEntryTest(long userId, SubmitEntryTestDto dto);
        Task<EntryTestResultModel> GetUserTestResult(long userId, long entryTestId);
    }
}
