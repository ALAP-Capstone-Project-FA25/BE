using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class TopicBizLogic : ITopicBizLogic
    {
        private readonly ITopicRepository _topicRepository;

        public TopicBizLogic(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<bool> CreateUpdateTopic(CreateUpdateTopicDto dto)
        {

            if (dto.Id > 0)
            {
                var topicModel = new TopicModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    OrderIndex = dto.OrderIndex,
                    CourseId = dto.CourseId,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _topicRepository.Update(topicModel);
            }
            else
            {
                var topicModel = new TopicModel
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    OrderIndex = dto.OrderIndex,
                    CourseId = dto.CourseId
                };
                return await _topicRepository.Create(topicModel);
            }
        }

        public async Task<bool> DeleteTopic(long id)
        {
            return await _topicRepository.Delete(id);
        }

        public async Task<TopicModel> GetTopicById(long id)
        {
            return await _topicRepository
                
                .GetById(id) ?? throw new KeyNotFoundException($"Không tìm thấy chủ đề.");
        }

        public async Task<PagedResult<TopicModel>> GetListTopicsByPaging(PagingModel pagingModel)
        {
            return await _topicRepository.GetListByPaging(pagingModel);
        }

        public async Task<PagedResult<UserTopicDto>> GetListTopicsByPagingByStudent(PagingModel pagingModel)
        {
            return await _topicRepository.GetListByPagingByUser(pagingModel);

        }
    }
}
