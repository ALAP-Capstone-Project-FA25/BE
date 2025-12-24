using App.Entity.DTO.Response;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface ITopicRepository
    {
        Task<bool> Create(TopicModel topicModel);
        Task<bool> Update(TopicModel topicModel);
        Task<TopicModel?> GetById(long id);
        Task<PagedResult<TopicModel>> GetListByPaging(PagingModel pagingModel);
        Task<PagedResult<UserTopicDto>> GetListByPagingByUser(PagingModel pagingModel);

        Task<bool> Delete(long id);
    }
}
