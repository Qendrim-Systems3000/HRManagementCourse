using AutoMapper;
using HRManagement.Application.DTOs;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Mapping;

public class EmployeeCourseMappingProfile : Profile
{
    public EmployeeCourseMappingProfile()
    {
        CreateMap<EmployeeCourse, EmployeeCourseResponseDto>()
            .ForMember(d => d.CourseDescription, opt => opt.MapFrom(s => s.Course != null ? s.Course.Description : ""));

        CreateMap<EmployeeCourse, EmployeeCourseTranscriptDto>()
            .ForMember(d => d.CourseDescription, opt => opt.MapFrom(s => s.Course != null ? s.Course.Description : ""));

        CreateMap<EnrollEmployeeDto, EmployeeCourse>();
        CreateMap<UpdateEmployeeCourseDto, EmployeeCourse>()
            .ForMember(d => d.EmployeeCourseId, opt => opt.Ignore())
            .ForMember(d => d.EmployeeId, opt => opt.Ignore())
            .ForMember(d => d.CourseId, opt => opt.Ignore())
            .ForMember(d => d.Employee, opt => opt.Ignore())
            .ForMember(d => d.Course, opt => opt.Ignore());
    }
}
