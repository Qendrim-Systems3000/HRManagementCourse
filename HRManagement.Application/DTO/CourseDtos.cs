namespace HRManagement.Application.DTOs;

public record CourseResponseDto(
    int CourseId,
    string Description,
    string CourseTypeName,
    DateTime StartDate,
    DateTime EndDate,
    int Hours,
    bool Approved
);

public record CreateCourseDto(
    string Description,
    int CourseTypeId,
    DateTime StartDate,
    DateTime EndDate,
    int Hours,
    int Credits,
    decimal DistrictCost,
    decimal EmployeeCost
);

public record UpdateCourseDto(
    string Description,
    int CourseTypeId,
    DateTime StartDate,
    DateTime EndDate,
    int Hours,
    int Credits,
    decimal DistrictCost,
    decimal EmployeeCost
);