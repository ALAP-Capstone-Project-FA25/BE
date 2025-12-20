using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class LessonBizLogic : ILessonBizLogic
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonBizLogic(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<bool> CreateUpdateLesson(CreateUpdateLessonDto dto)
        {
            var lastOrderIndex = await _lessonRepository.GetLastOrderIndexByTopic(dto.TopicId);

            if (dto.Id > 0)
            {
                var existingLesson = await _lessonRepository.GetById(dto.Id);
                if (existingLesson == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy bài học.");
                }

                existingLesson.Title = dto.Title;
                existingLesson.Description = dto.Description;
                existingLesson.Content = dto.Content;
                existingLesson.VideoUrl = dto.VideoUrl;
                existingLesson.Duration = dto.Duration;
                existingLesson.OrderIndex = dto.OrderIndex;
                existingLesson.IsFree = dto.IsFree;
                existingLesson.TopicId = dto.TopicId;
                existingLesson.LessonType = (LessonType)dto.LessonType;
                existingLesson.DocumentUrl = dto.DocumentUrl;
                existingLesson.DocumentContent = dto.DocumentContent;
                existingLesson.UpdatedAt = Utils.GetCurrentVNTime();

                return await _lessonRepository.Update(existingLesson);
            }
            else
            {
                var lessonModel = new LessonModel
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Content = dto.Content,
                    VideoUrl = dto.VideoUrl,
                    Duration = dto.Duration,
                    OrderIndex = lastOrderIndex + 1,
                    IsFree = dto.IsFree,
                    TopicId = dto.TopicId,
                    LessonType = (LessonType)dto.LessonType,
                    DocumentUrl = dto.DocumentUrl,
                    DocumentContent = dto.DocumentContent
                };
                return await _lessonRepository.Create(lessonModel);
            }
        }

        public async Task<bool> DeleteLesson(long id)
        {
            return await _lessonRepository.Delete(id);
        }

        public async Task<LessonModel> GetLessonById(long id)
        {
            return await _lessonRepository.GetById(id) ?? throw new KeyNotFoundException($"Không tìm thấy bài học.");
        }

        public async Task<PagedResult<LessonModel>> GetListLessonsByPaging(PagingModel pagingModel, long t)
        {
            return await _lessonRepository.GetListByPaging(pagingModel, t);
        }
    }
}
