using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class UserLessonProgressBizLogic : IUserLessonProgressBizLogic
    {
        private readonly IUserLessonRepository _userLessonRepository;
        private readonly IUserTopicRepository _userTopicRepository;
        private readonly IUserCourseRepository _userCourseRepository;

        public UserLessonProgressBizLogic(IUserLessonRepository userLessonRepository, IUserTopicRepository userTopicRepository, IUserCourseRepository userCourseRepository)
        {
            _userLessonRepository = userLessonRepository;
            _userTopicRepository = userTopicRepository;
            _userCourseRepository = userCourseRepository;
        }

        private async Task<long> ResolveUserTopicId(long userId, long courseId, long topicId)
        {
            var uc = await _userCourseRepository.Get(userId, courseId) ?? throw new KeyNotFoundException("Chưa đăng ký khóa học");
            var ut = await _userTopicRepository.Get(uc.Id, topicId) ?? throw new KeyNotFoundException("Chưa có tiến độ topic");
            return ut.Id;
        }

        public async Task<UserLessonModel?> Get(long userId, long courseId, long topicId, long lessonId)
        {
            var userTopicId = await ResolveUserTopicId(userId, courseId, topicId);
            return await _userLessonRepository.Get(userTopicId, lessonId);
        }

        public async Task<List<UserLessonModel>> GetByTopic(long userId, long courseId, long topicId)
        {
            var userTopicId = await ResolveUserTopicId(userId, courseId, topicId);
            return await _userLessonRepository.GetByUserTopic(userTopicId);
        }

        public async Task<bool> CreateOrUpdate(long userId, CreateUpdateUserLessonDto dto)
        {
            var userTopicId = await ResolveUserTopicId(userId, dto.CourseId, dto.TopicId);
            var model = await _userLessonRepository.Get(userTopicId, dto.LessonId);
            if (model == null)
            {
                model = new UserLessonModel
                {
                    UserTopicId = userTopicId,
                    LessonId = dto.LessonId,
                    IsDone = dto.IsCompleted
                };
                return await _userLessonRepository.Create(model);
            }
            model.IsDone = dto.IsCompleted;
            var updated = await _userLessonRepository.Update(model);

            // propagate topic progress and unlock next topic if needed
            var userLessons = await _userLessonRepository.GetByUserTopic(userTopicId);
            var total = userLessons.Count;
            if (total > 0)
            {
                var done = userLessons.Count(x => x.IsDone);
                var percent = (int)System.Math.Round((double)done * 100 / total);

                var userTopic = await _userTopicRepository.GetByUserCourse((await _userCourseRepository.Get(userId, dto.CourseId))!.Id)
                    .ContinueWith(t => t.Result.First(x => x.Id == userTopicId));
                userTopic.Progress = percent;
                await _userTopicRepository.Update(userTopic);

                // if topic completed → unlock next topic
                if (percent >= 100)
                {
                    var userCourseId = userTopic.UserCourseId;
                    var topics = await _userTopicRepository.GetByUserCourse(userCourseId);
                    var ordered = topics.OrderBy(t => t.Idx).ToList();
                    var idx = ordered.FindIndex(t => t.Id == userTopicId);
                    if (idx >= 0 && idx + 1 < ordered.Count)
                    {
                        var next = ordered[idx + 1];
                        if (next.IsLocked)
                        {
                            next.IsLocked = false;
                            await _userTopicRepository.Update(next);
                        }
                    }

                    // check course completion
                    var allDone = ordered.All(t => t.Progress >= 100);
                    if (allDone)
                    {
                        var uc = await _userCourseRepository.Get(userId, dto.CourseId);
                        if (uc != null)
                        {
                            uc.IsDone = true;
                            uc.CompletedAt = Base.Common.Utils.GetCurrentVNTime();
                            await _userCourseRepository.Update(uc);
                        }
                    }
                }
            }

            return updated;
        }

        public async Task<object> GetUserLessonProgress(long userId, long lessonId)
        {
            // TODO: Implement proper video progress tracking
            // For now, return mock data structure
            return new
            {
                TotalDuration = 0,
                WatchedDuration = 0,
                CurrentTime = 0,
                WatchedRanges = new object[0],
                IsPlaying = false,
                LastUpdated = System.DateTime.UtcNow
            };
        }
    }
}


