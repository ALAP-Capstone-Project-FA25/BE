using ALAP.BLL;
using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.Entity.DTO.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class SearchBizLogic : AppBaseBizLogic, ISearchBizLogic
    {
        public SearchBizLogic(BaseDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<SearchResultDto> SearchAll(string keyword, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new SearchResultDto();
            }

            var searchTerm = keyword.Trim();

            // Search Courses
            var courses = await _dbContext.Courses
                .Include(c => c.Category)
                .Where(c => c.Title.Contains(searchTerm) ||
                           (c.Description != null && c.Description.Contains(searchTerm)))
                .Take(limit)
                .Select(c => new CourseSearchResultDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description ?? string.Empty,
                    ImageUrl = c.ImageUrl ?? string.Empty,
                    CategoryName = c.Category != null ? c.Category.Name : string.Empty
                })
                .ToListAsync();

            // Search Lessons
            var lessons = await _dbContext.Lessons
                .Include(l => l.Topic)
                    .ThenInclude(t => t.Course)
                .Where(l => l.Title.Contains(searchTerm) ||
                           (l.Description != null && l.Description.Contains(searchTerm)))
                .Take(limit)
                .Select(l => new LessonSearchResultDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description ?? string.Empty,
                    VideoUrl = l.VideoUrl ?? string.Empty,
                    CourseTitle = l.Topic != null && l.Topic.Course != null ? l.Topic.Course.Title : string.Empty,
                    TopicTitle = l.Topic != null ? l.Topic.Title : string.Empty,
                    CourseId = l.Topic != null && l.Topic.Course != null ? l.Topic.Course.Id : 0,
                    TopicId = l.TopicId
                })
                .ToListAsync();

            // Search Topics
            var topics = await _dbContext.Topics
                .Include(t => t.Course)
                .Where(t => t.Title.Contains(searchTerm) ||
                           (t.Description != null && t.Description.Contains(searchTerm)))
                .Take(limit)
                .Select(t => new TopicSearchResultDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description ?? string.Empty,
                    CourseTitle = t.Course != null ? t.Course.Title : string.Empty,
                    CourseId = t.CourseId
                })
                .ToListAsync();

            return new SearchResultDto
            {
                Courses = courses,
                Lessons = lessons,
                Topics = topics,
                TotalCount = courses.Count + lessons.Count + topics.Count
            };
        }
    }
}
