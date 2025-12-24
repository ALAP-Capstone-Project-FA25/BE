using App.DAL.DataBase;
using App.DAL.Interface;
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
    public class CourseRepository : AppBaseRepository, ICourseRepository
    {
        private readonly BaseDBContext _dbContext;

        public CourseRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(CourseModel courseModel)
        {
            await _dbContext.Courses.AddAsync(courseModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var existingCourse = await _dbContext.Courses.FindAsync(id);
            if (existingCourse == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy khóa học.");
            }
            _dbContext.Courses.Remove(existingCourse);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<CourseModel?> GetById(long id)
        {
            var course = await _dbContext.Courses
                .Include(c => c.Category)
                .Include(c => c.Topics)
                    .ThenInclude(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
            return course;
        }

        public async Task<PagedResult<CourseModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.Courses
                .Include(c => c.Category)
                .Include(c => c.Mentor)
                .Include(c => c.UserCourses)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(c => c.Title.Contains(pagingModel.Keyword) || c.Description.Contains(pagingModel.Keyword));
            }

            if(pagingModel.UserId > 0)
            {
               query = query.Where(c => c.MentorId == pagingModel.UserId);
            }

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<bool> Update(CourseModel courseModel)
        {
            _dbContext.Courses.Update(courseModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
