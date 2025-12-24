using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Response;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
    public class TopicRepository : AppBaseRepository, ITopicRepository
    {
        private readonly BaseDBContext _dbContext;

        public TopicRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(TopicModel topicModel)
        {
            await _dbContext.Topics.AddAsync(topicModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var existingTopic = await _dbContext.Topics.FindAsync(id);
            if (existingTopic == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy chủ đề.");
            }
            _dbContext.Topics.Remove(existingTopic);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<TopicModel?> GetById(long id)
        {
            var topic = await _dbContext.Topics
                .Include(t => t.Course)
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(t => t.Id == id);
            return topic;
        }

        public async Task<PagedResult<TopicModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.Topics
                .Include(t => t.Course)
                .Include(t => t.Lessons)
                .Include(t => t.TopicQuestions)
                    .ThenInclude(ta => ta.TopicQuestionAnswers)
                .Where(x => x.CourseId == pagingModel.CourseId)
                .AsQueryable();
            

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(t => t.Title.Contains(pagingModel.Keyword) || t.Description.Contains(pagingModel.Keyword));
            }
            
            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<PagedResult<UserTopicDto>> GetListByPagingByUser(PagingModel pagingModel)
        {
            var query = _dbContext.Topics
                .Include(t => t.Course)
                .Include(t => t.Lessons)
                .Include(t => t.TopicQuestions)
                    .ThenInclude(ta => ta.TopicQuestionAnswers)
                .Where(x => x.CourseId == pagingModel.CourseId)
                .AsQueryable();


            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(t => t.Title.Contains(pagingModel.Keyword) || t.Description.Contains(pagingModel.Keyword));
            }

            var userCourse = await _dbContext.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == pagingModel.UserId && uc.CourseId == pagingModel.CourseId);

            long currentTopicId = userCourse != null ? userCourse.CurrentTopicId : 0;
            long currentLessonId = userCourse != null ? userCourse.CurrentLessonId : 0;



            PagedResult<TopicModel> topics =  await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
            
            // Get UserTopics for this userCourse
            var userTopics = userCourse != null 
                ? await _dbContext.UserTopics
                    .Where(ut => ut.UserCourseId == userCourse.Id)
                    .ToListAsync()
                : new List<UserTopicModel>();

            var dtos = topics.ListObjects
                .OrderBy(t => t.OrderIndex)
                .Select(topic => 
                {
                    var userTopic = userTopics.FirstOrDefault(ut => ut.TopicId == topic.Id);
                    return new UserTopicDto
                    {
                        Id = topic.Id,
                        Title = topic.Title,
                        Description = topic.Description,
                        OrderIndex = topic.OrderIndex,
                        CourseId = topic.CourseId,
                        IsCurrent = currentTopicId == topic.Id,
                        UserTopicId = userTopic?.Id,
                        CreatedAt = topic.CreatedAt,
                        UpdatedAt = topic.UpdatedAt,
                        Lessons = topic.Lessons
                            .OrderBy(x => x.OrderIndex)
                            .Select(x => new UserLessonDto
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Description = x.Description,
                                Content = x.Content,
                                VideoUrl = x.VideoUrl,
                                LessonType = x.LessonType,
                                DocumentContent = x.DocumentContent,
                                DocumentUrl = x.DocumentUrl,
                                Duration = x.Duration,
                                OrderIndex = x.OrderIndex,
                                IsFree = x.IsFree,
                                IsCurrent = currentLessonId == x.Id,
                                TopicId = x.TopicId
                            })
                            .ToList(),
                        TopicQuestions = topic.TopicQuestions.ToList()
                    };
                })
                .ToList();

            return new PagedResult<UserTopicDto>(
         dtos,
         topics.TotalRecords,
         topics.PageNumber,
         topics.PageSize
     );


        }

        public async Task<bool> Update(TopicModel topicModel)
        {
            _dbContext.Topics.Update(topicModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
