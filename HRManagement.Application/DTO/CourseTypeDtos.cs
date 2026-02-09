namespace HRManagement.Application.DTOs;

public record CourseTypeResponseDto(int CourseTypeId, string Description);

public record CreateCourseTypeDto(string Description);

public record UpdateCourseTypeDto(string Description);
