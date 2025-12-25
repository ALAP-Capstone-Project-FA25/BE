using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using AutoMapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using PayOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ALAP.BLL.Implement
{
    public class CourseBizLogic : AppBaseBizLogic, ICourseBizLogic
    {
        private readonly ICourseRepository _courseRepository;
        private readonly BaseDBContext _dbContext;
        private readonly IMapper _mapper;

        public CourseBizLogic(BaseDBContext dbContext, ICourseRepository courseRepository, IMapper mapper) : base(dbContext)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> AssignMentorToCourse(long courseId, long mentorId)
        {
            var course = await _courseRepository.GetById(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException("Khóa học không tồn tại.");
            }
            course.MentorId = mentorId;
            return await _courseRepository.Update(course);

        }

        public async Task<bool> CreateUpdateCourse(CreateUpdateCourseDto dto)
        {
            if (dto.Id > 0)
            {
                var courseModel = new CourseModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    ImageUrl = dto.ImageUrl,
                    Description = dto.Description,
                    Price = dto.Price,
                    SalePrice = dto.SalePrice,
                    MentorId = dto.MentorId,
                    CategoryId = dto.CategoryId,
                    CourseType = dto.CourseType,
                    Difficulty = dto.Difficulty,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _courseRepository.Update(courseModel);
            }
            else
            {
                var courseModel = new CourseModel
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price,
                    ImageUrl = dto.ImageUrl,
                    MentorId = dto.MentorId,
                    SalePrice = dto.SalePrice,
                    CategoryId = dto.CategoryId,
                    CourseType = dto.CourseType,
                    Difficulty = dto.Difficulty,
                    Members = 0
                };
                return await _courseRepository.Create(courseModel);
            }
        }

        public async Task<bool> DeleteCourse(long id)
        {
            return await _courseRepository.Delete(id);
        }

        public async Task<CourseModel> GetCourseById(long id)
        {
            return await _courseRepository.GetById(id) ?? throw new KeyNotFoundException($"Không tìm thấy khóa học.");
        }

        public Task<List<CourseModel>> GetListCourseByCategoryId(long categoryId)
        {
            var courses = _dbContext.Courses
                .Where(c => c.CategoryId == categoryId)
                .ToListAsync();
            return courses;
        }

        public async Task<PagedResult<CourseDto>> GetListCoursesByPaging(PagingModel pagingModel)
        {
            var courses = await _courseRepository.GetListByPaging(pagingModel);

            var courseDtos = _mapper.Map<List<CourseDto>>(courses.ListObjects);

            var pagedResult = new PagedResult<CourseDto>(
                courseDtos,
                courses.TotalRecords,
                courses.PageNumber,
                courses.PageSize
            );

            return pagedResult;
        }

    }
}
