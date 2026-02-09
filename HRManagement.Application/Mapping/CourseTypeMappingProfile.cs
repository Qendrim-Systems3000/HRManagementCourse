using AutoMapper;
using HRManagement.Application.DTOs;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Mapping;

public class CourseTypeMappingProfile : Profile
{
    public CourseTypeMappingProfile()
    {
        CreateMap<CourseType, CourseTypeResponseDto>()
            .ConstructUsing(ct => new CourseTypeResponseDto(ct.CourseTypeId, ct.Description));
        CreateMap<CreateCourseTypeDto, CourseType>();
        CreateMap<UpdateCourseTypeDto, CourseType>()
            .ForMember(d => d.CourseTypeId, opt => opt.Ignore())
            .ForMember(d => d.Courses, opt => opt.Ignore());
    }
}
