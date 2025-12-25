using ALAP.Entity.DTO;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using AutoMapper;

namespace ALAP.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserModel, UserResponseDTO>().ReverseMap();

            CreateMap<CategoryDto, CategoryModel>().ReverseMap();
            CreateMap<CourseModel, CourseDto>().ReverseMap();
            CreateMap<TopicModel, UserTopicDto>().ReverseMap();
            CreateMap<LessonModel, UserLessonDto>().ReverseMap();
            CreateMap<UserCourseModel, UserCourseDto>().ReverseMap();

            CreateMap<CourseModel, CourseDto>()
            .ForMember(dest => dest.Topics, opt => opt.Ignore())
            .ForMember(dest => dest.UserCourses, opt => opt.Ignore());
        }
    }
}
