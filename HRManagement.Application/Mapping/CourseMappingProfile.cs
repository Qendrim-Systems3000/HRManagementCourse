using AutoMapper;
using HRManagement.Application.DTOs;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Mapping;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseResponseDto>()
            .ConstructUsing(c => new CourseResponseDto(
                c.CourseId,
                c.Description,
                c.CourseType != null ? c.CourseType.Description : "N/A",
                c.StartDate,
                c.EndDate,
                c.Hours,
                c.Approved));

        CreateMap<CreateCourseDto, Course>();
        CreateMap<UpdateCourseDto, Course>()
            .ForMember(d => d.CourseId, opt => opt.Ignore())
            .ForMember(d => d.CourseType, opt => opt.Ignore())
            .ForMember(d => d.EmployeeCourses, opt => opt.Ignore())
            .ForMember(d => d.TuitionEligible, opt => opt.Ignore())
            .ForMember(d => d.Approved, opt => opt.Ignore())
            .ForMember(d => d.MaintenanceOfLicense, opt => opt.Ignore())
            .ForMember(d => d.Provider, opt => opt.Ignore())
            .ForMember(d => d.Presenter, opt => opt.Ignore())
            .ForMember(d => d.Institution, opt => opt.Ignore())
            .ForMember(d => d.Degree, opt => opt.Ignore())
            .ForMember(d => d.CertNo, opt => opt.Ignore())
            .ForMember(d => d.Location, opt => opt.Ignore())
            .ForMember(d => d.Notes, opt => opt.Ignore());
    }
}
