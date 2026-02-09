using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Application.DTOs;

namespace HRManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeeCoursesController : ControllerBase
{
    private readonly IEmployeeCourseService _service;

    public EmployeeCoursesController(IEmployeeCourseService service)
    {
        _service = service;
    }

    [HttpPost("enroll")]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Enroll(EnrollEmployeeDto dto)
    {
        var enrollment = new EmployeeCourse
        {
            EmployeeId = dto.EmployeeId,
            CourseId = dto.CourseId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Hours = dto.Hours,
            Credits = dto.Credits,
            DistrictCost = dto.DistrictCost,
            EmployeeCost = dto.EmployeeCost,
            Grade = dto.Grade,
            Major = dto.Major,
            Notes = dto.Notes
        };

        try
        {
            await _service.EnrollEmployeeAsync(enrollment);
            return Ok(new { Message = "Employee enrolled in course successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); // If Employee/Course doesn't exist or belongs to another district
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> List([FromQuery] int? employeeId, [FromQuery] int? courseId)
    {
        var list = await _service.GetEnrollmentsAsync(employeeId, courseId);
        var response = list.Select(ec => new EmployeeCourseResponseDto(
            ec.EmployeeCourseId,
            ec.EmployeeId,
            ec.CourseId,
            ec.Course?.Description ?? "",
            ec.StartDate,
            ec.EndDate,
            ec.Hours,
            ec.Credits,
            ec.DistrictCost,
            ec.EmployeeCost,
            ec.Grade,
            ec.Major,
            ec.Notes
        ));
        return Ok(response);
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<IActionResult> GetTranscript(int employeeId)
    {
        var transcript = await _service.GetEmployeeTranscriptAsync(employeeId);
        var response = transcript.Select(ec => new EmployeeCourseTranscriptDto(
            ec.EmployeeCourseId,
            ec.EmployeeId,
            ec.Course?.Description ?? "",
            ec.StartDate,
            ec.Grade
        ));
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdateEmployeeCourseDto dto)
    {
        var enrollment = await _service.GetEnrollmentByIdAsync(id);
        if (enrollment == null) return NotFound();

        enrollment.StartDate = dto.StartDate;
        enrollment.EndDate = dto.EndDate;
        enrollment.Hours = dto.Hours;
        enrollment.Credits = dto.Credits;
        enrollment.DistrictCost = dto.DistrictCost;
        enrollment.EmployeeCost = dto.EmployeeCost;
        enrollment.Grade = dto.Grade;
        enrollment.Major = dto.Major;
        enrollment.Notes = dto.Notes;

        await _service.UpdateEnrollmentAsync(enrollment);
        return Ok(new { Message = "Enrollment updated successfully." });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var enrollment = await _service.GetEnrollmentByIdAsync(id);
        if (enrollment == null) return NotFound();

        await _service.RemoveEnrollmentAsync(id);
        return NoContent();
    }
}