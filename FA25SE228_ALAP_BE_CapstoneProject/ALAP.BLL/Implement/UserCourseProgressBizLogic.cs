using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class UserCourseProgressBizLogic : IUserCourseProgressBizLogic
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserCourseRepository _userCourseRepository;
        private readonly IUserTopicRepository _userTopicRepository;
        private readonly IUserLessonRepository _userLessonRepository;
        private readonly ITopicBizLogic _topicBizLogic;
        private readonly IMapper _mapper;

        public UserCourseProgressBizLogic(ICourseRepository courseRepository, 
            IUserCourseRepository userCourseRepository, 
            IUserTopicRepository userTopicRepository, IUserLessonRepository userLessonRepository,
            ITopicBizLogic topicBizLogic,
            IMapper mapper
            )
        {
            _courseRepository = courseRepository;
            _userCourseRepository = userCourseRepository;
            _userTopicRepository = userTopicRepository;
            _userLessonRepository = userLessonRepository;
            _topicBizLogic = topicBizLogic;
            _mapper = mapper;
        }

        public async Task<bool> Enroll(long userId, long courseId)
        {
            var existing = await _userCourseRepository.Get(userId, courseId);
            if (existing != null)
            {
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    return await _userCourseRepository.Update(existing);
                }
                return true;
            }

            var course = await _courseRepository.GetById(courseId) ?? throw new KeyNotFoundException("Khóa học không tồn tại");
            var model = new UserCourseModel
            {
                UserId = userId,
                CourseId = courseId,
                Title = course.Title,
                Description = course.Description,
                IsActive = true,
                IsDone = false
            };
            var ok = await _userCourseRepository.Enroll(model);
            if (!ok) return false;

            // Snapshot topics and lessons
            var topics = course.Topics.OrderBy(t => t.OrderIndex).ToList();
            for (int i = 0; i < topics.Count; i++)
            {
                var topic = topics[i];
                var userTopic = new UserTopicModel
                {
                    UserCourseId = model.Id,
                    TopicId = topic.Id,
                    Idx = topic.OrderIndex,
                    IsLocked = i > 0,
                    Progress = 0
                };
                await _userTopicRepository.Create(userTopic);

                var lessons = topic.Lessons.OrderBy(l => l.OrderIndex).ToList();
                foreach (var lesson in lessons)
                {
                    var userLesson = new UserLessonModel
                    {
                        UserTopicId = userTopic.Id,
                        LessonId = lesson.Id,
                        IsDone = false
                    };
                    await _userLessonRepository.Create(userLesson);
                }
            }
            return true;
        }

        public async Task<UserCourseModel?> Get(long userId, long courseId)
        {
            return await _userCourseRepository.Get(userId, courseId);
        }

        public async Task<List<UserCourseDto>> GetListUserCourseByCourseId(long courseId)
        {
            var userCourses = await _userCourseRepository.GetListUserCourseByCourseId(courseId);
            return _mapper.Map<List<UserCourseDto>>(userCourses);
        }

        public async Task<List<UserCourseModel>> GetMyCourses(long userId)
        {
            return await _userCourseRepository.GetByUser(userId);
        }

        public async Task<bool> Update(long userId, UpdateUserCourseDto dto)
        {
            var model = await _userCourseRepository.Get(userId, dto.CourseId) ?? throw new KeyNotFoundException("Chưa đăng ký khóa học");
            if (dto.CurrentTopicId.HasValue) { /* no-op: field removed in ERD snapshot; can compute via UserTopic */ }
            if (dto.CurrentLessonId.HasValue) { /* no-op: field removed */ }
            if (dto.IsCompleted.HasValue)
            {
                model.IsDone = dto.IsCompleted.Value;
                model.CompletedAt = dto.IsCompleted.Value ? Base.Common.Utils.GetCurrentVNTime() : null;
            }
            return await _userCourseRepository.Update(model);
        }

        public async Task<bool> UpdateLessonProgress(long topicId, long lessonId, long userId)
        {
            var topic = await _topicBizLogic.GetTopicById(topicId);
            var userCourse = await _userCourseRepository.Get(userId, topic.CourseId);
            if (userCourse == null)
            {
                throw new KeyNotFoundException("Chưa đăng ký khóa học");
            }
            long currentTopicId = userCourse.CurrentTopicId;
            long currentLessonId = userCourse.CurrentLessonId;

            if (lessonId > currentLessonId) {
                userCourse.CurrentTopicId = topicId;
                userCourse.CurrentLessonId = lessonId;


                var course = await _courseRepository.GetById(topic.CourseId);

                var totalLessonsInCourse = course.Topics.Sum(t => t.Lessons.Count);

                var topics = course.Topics.OrderBy(t => t.OrderIndex).ToList();

                int doneLessonsBeforeCurrentTopic = topics
                    .Where(t => t.OrderIndex < topic.OrderIndex)
                    .Sum(t => t.Lessons.Count);

                var currentTopicLessons = topic.Lessons.OrderBy(l => l.OrderIndex).ToList();
                var currentLessonIndex = currentTopicLessons.FindIndex(l => l.Id == lessonId); 
                var doneInCurrentTopic = currentLessonIndex;

                var totalDoneLessons = doneLessonsBeforeCurrentTopic + doneInCurrentTopic;

                double percent = 0;
                if (totalLessonsInCourse > 0)
                {
                    percent = (double)totalDoneLessons / totalLessonsInCourse * 100.0;
                }

                userCourse.ProgressPercent = percent;
                await _userCourseRepository.Update(userCourse);

            }
            return true;
        }

        public async Task<object> GetStudentProgress(long userId, long courseId)
        {
            var userCourse = await _userCourseRepository.Get(userId, courseId);
            if (userCourse == null)
            {
                return new
                {
                    IsEnrolled = false,
                    Message = "Học viên chưa đăng ký khóa học này"
                };
            }

            var course = await _courseRepository.GetById(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException("Khóa học không tồn tại");
            }

            var topics = course.Topics.OrderBy(t => t.OrderIndex).ToList();
            var totalLessons = topics.Sum(t => t.Lessons.Count);
            var completedLessons = 0;

            var topicsProgress = new List<object>();

            foreach (var topic in topics)
            {
                var lessons = topic.Lessons.OrderBy(l => l.OrderIndex).ToList();
                var topicCompletedLessons = 0;

                // Check if current lesson is in this topic
                var isCurrentTopic = topic.Id == userCourse.CurrentTopicId;
                var currentLessonInTopic = isCurrentTopic ? userCourse.CurrentLessonId : 0;

                foreach (var lesson in lessons)
                {
                    // Lesson is completed if it's before or equal to current lesson in current topic
                    var isCompleted = false;
                    if (topic.OrderIndex < topics.FirstOrDefault(t => t.Id == userCourse.CurrentTopicId)?.OrderIndex)
                    {
                        // All lessons in previous topics are completed
                        isCompleted = true;
                    }
                    else if (isCurrentTopic && lesson.Id <= currentLessonInTopic)
                    {
                        isCompleted = true;
                    }

                    if (isCompleted)
                    {
                        topicCompletedLessons++;
                        completedLessons++;
                    }
                }

                topicsProgress.Add(new
                {
                    TopicId = topic.Id,
                    TopicTitle = topic.Title,
                    TotalLessons = lessons.Count,
                    CompletedLessons = topicCompletedLessons,
                    Progress = lessons.Count > 0 ? (double)topicCompletedLessons / lessons.Count * 100 : 0
                });
            }

            return new
            {
                IsEnrolled = true,
                UserId = userId,
                CourseId = courseId,
                CourseTitle = course.Title,
                ProgressPercent = userCourse.ProgressPercent,
                TotalLessons = totalLessons,
                CompletedLessons = completedLessons,
                CurrentTopicId = userCourse.CurrentTopicId,
                CurrentLessonId = userCourse.CurrentLessonId,
                IsDone = userCourse.IsDone,
                CompletedAt = userCourse.CompletedAt,
                TopicsProgress = topicsProgress
            };
        }

        public async Task<object> GetCurrentLesson(long userId, long courseId)
        {
            var userCourse = await _userCourseRepository.Get(userId, courseId);
            if (userCourse == null)
            {
                return new
                {
                    IsEnrolled = false,
                    Message = "Học viên chưa đăng ký khóa học này"
                };
            }

            if (userCourse.CurrentLessonId == 0)
            {
                return new
                {
                    IsEnrolled = true,
                    HasStarted = false,
                    Message = "Học viên chưa bắt đầu học"
                };
            }

            var course = await _courseRepository.GetById(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException("Khóa học không tồn tại");
            }

            var currentTopic = course.Topics.FirstOrDefault(t => t.Id == userCourse.CurrentTopicId);
            var currentLesson = currentTopic?.Lessons.FirstOrDefault(l => l.Id == userCourse.CurrentLessonId);

            if (currentLesson == null)
            {
                return new
                {
                    IsEnrolled = true,
                    HasStarted = false,
                    Message = "Không tìm thấy bài học hiện tại"
                };
            }

            return new
            {
                IsEnrolled = true,
                HasStarted = true,
                CurrentLesson = new
                {
                    LessonId = currentLesson.Id,
                    LessonTitle = currentLesson.Title,
                    LessonType = currentLesson.LessonType,
                    VideoUrl = currentLesson.VideoUrl,
                    DocumentUrl = currentLesson.DocumentUrl,
                    Duration = currentLesson.Duration,
                    TopicId = currentTopic.Id,
                    TopicTitle = currentTopic.Title
                },
                ProgressPercent = userCourse.ProgressPercent
            };
        }
    }
}


