using AutoMapper;
using HRManagement.Application.DTOs;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Mapping;

public class EmployeeCourseMappingProfile : Profile
{
    public EmployeeCourseMappingProfile()
    {
        CreateMap<EmployeeCourse, EmployeeCourseResponseDto>()
            .ConstructUsing(ec => new EmployeeCourseResponseDto(
                ec.EmployeeCourseId,
                ec.EmployeeId,
                ec.CourseId,
                ec.Course != null ? ec.Course.Description : "",
                ec.StartDate,
                ec.EndDate,
                ec.Hours,
                ec.Credits,
                ec.DistrictCost,
                ec.EmployeeCost,
                ec.Grade,
                ec.Major,
                ec.Notes));

        CreateMap<EmployeeCourse, EmployeeCourseTranscriptDto>()
            .ConstructUsing(ec => new EmployeeCourseTranscriptDto(
                ec.EmployeeCourseId,
                ec.EmployeeId,
                ec.Course != null ? ec.Course.Description : "",
                ec.StartDate,
                ec.Grade));

        CreateMap<EnrollEmployeeDto, EmployeeCourse>();
        CreateMap<UpdateEmployeeCourseDto, EmployeeCourse>()
            .ForMember(d => d.EmployeeCourseId, opt => opt.Ignore())
            .ForMember(d => d.EmployeeId, opt => opt.Ignore())
            .ForMember(d => d.CourseId, opt => opt.Ignore())
            .ForMember(d => d.Employee, opt => opt.Ignore())
            .ForMember(d => d.Course, opt => opt.Ignore());
    }
}
