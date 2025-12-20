using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class UserTopicProgressBizLogic : IUserTopicProgressBizLogic
    {
        private readonly IUserTopicRepository _userTopicRepository;
        private readonly IUserCourseRepository _userCourseRepository;

        public UserTopicProgressBizLogic(IUserTopicRepository userTopicRepository, IUserCourseRepository userCourseRepository)
        {
            _userTopicRepository = userTopicRepository;
            _userCourseRepository = userCourseRepository;
        }

        private async Task<long> GetUserCourseId(long userId, long courseId)
        {
            var uc = await _userCourseRepository.Get(userId, courseId) ?? throw new KeyNotFoundException("Chưa đăng ký khóa học");
            return uc.Id;
        }

        public async Task<UserTopicModel?> Get(long userId, long courseId, long topicId)
        {
            var userCourseId = await GetUserCourseId(userId, courseId);
            return await _userTopicRepository.Get(userCourseId, topicId);
        }

        public async Task<List<UserTopicModel>> GetByCourse(long userId, long courseId)
        {
            var userCourseId = await GetUserCourseId(userId, courseId);
            return await _userTopicRepository.GetByUserCourse(userCourseId);
        }

        public async Task<bool> CreateOrUpdate(long userId, CreateUpdateUserTopicDto dto)
        {
            var userCourseId = await GetUserCourseId(userId, dto.CourseId);
            var model = await _userTopicRepository.Get(userCourseId, dto.TopicId);
            if (model == null)
            {
                model = new UserTopicModel
                {
                    UserCourseId = userCourseId,
                    TopicId = dto.TopicId,
                    Idx = 0,
                    IsLocked = false,
                    Progress = 0
                };
                return await _userTopicRepository.Create(model);
            }
            if (dto.CurrentLessonId.HasValue)
            {
                // Quy đổi cập nhật progress theo lesson hiện tại (ở đây chỉ giữ trường progress int)
                // Có thể tính theo số lượng lesson done ngoài UserLesson nếu cần
            }
            if (dto.IsCompleted.HasValue && dto.IsCompleted.Value)
            {
                model.Progress = 100;
            }
            return await _userTopicRepository.Update(model);
        }
    }
}


