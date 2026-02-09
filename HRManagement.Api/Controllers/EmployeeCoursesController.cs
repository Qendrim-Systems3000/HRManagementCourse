using AutoMapper;
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
    private readonly IMapper _mapper;

    public EmployeeCoursesController(IEmployeeCourseService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost("enroll")]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Enroll(EnrollEmployeeDto dto)
    {
        var enrollment = _mapper.Map<EmployeeCourse>(dto);
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
        var response = _mapper.Map<IEnumerable<EmployeeCourseResponseDto>>(list);
        return Ok(response);
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<IActionResult> GetTranscript(int employeeId)
    {
        var transcript = await _service.GetEmployeeTranscriptAsync(employeeId);
        var response = _mapper.Map<IEnumerable<EmployeeCourseTranscriptDto>>(transcript);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdateEmployeeCourseDto dto)
    {
        var enrollment = await _service.GetEnrollmentByIdAsync(id);
        if (enrollment == null) return NotFound();

        _mapper.Map(dto, enrollment);
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