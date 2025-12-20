using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface ITopicBizLogic
    {
        Task<bool> CreateUpdateTopic(CreateUpdateTopicDto dto);
        Task<TopicModel> GetTopicById(long id);
        Task<PagedResult<TopicModel>> GetListTopicsByPaging(PagingModel pagingModel);
        Task<PagedResult<UserTopicDto>> GetListTopicsByPagingByStudent(PagingModel pagingModel);

        Task<bool> DeleteTopic(long id);
    }
}
