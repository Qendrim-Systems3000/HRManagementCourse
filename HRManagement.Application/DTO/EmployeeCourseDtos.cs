namespace HRManagement.Application.DTOs;

public record EnrollEmployeeDto(
    int EmployeeId,
    int CourseId,
    DateTime StartDate,
    DateTime EndDate,
    int Hours,
    int Credits,
    decimal DistrictCost,
    decimal EmployeeCost,
    string? Grade,
    string? Major,
    string? Notes
);

public record EmployeeCourseResponseDto(
    int EmployeeCourseId,
    int EmployeeId,
    int CourseId,
    string CourseDescription,
    DateTime StartDate,
    DateTime EndDate,
    int Hours,
    int Credits,
    decimal DistrictCost,
    decimal EmployeeCost,
    string? Grade,
    string? Major,
    string? Notes
);

/// <summary>Simplified DTO for transcript view.</summary>
public record EmployeeCourseTranscriptDto(
    int EmployeeCourseId,
    int EmployeeId,
    string CourseDescription,
    DateTime StartDate,
    string? Grade
);

public record UpdateEmployeeCourseDto(
    DateTime StartDate,
    DateTime EndDate,
    int Hours,
    int Credits,
    decimal DistrictCost,
    decimal EmployeeCost,
    string? Grade,
    string? Major,
    string? Notes
);