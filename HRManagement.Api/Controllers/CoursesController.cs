using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Application.DTOs;

namespace HRManagement.Api.Controllers;

[Authorize] // Requires JWT
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? typeId, [FromQuery] DateTime? date, [FromQuery] bool? approved)
    {
        var courses = await _courseService.GetCoursesAsync(typeId, date, approved);
        
        // Mapping to DTO (In a real app, use AutoMapper)
        var response = courses.Select(c => new CourseResponseDto(
            c.CourseId, c.Description, c.CourseType?.Description ?? "N/A", c.StartDate, c.EndDate, c.Hours, c.Approved
        ));

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRUser")] // Role-based access
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        var course = new Course 
        { 
            Description = dto.Description,
            CourseTypeId = dto.CourseTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Hours = dto.Hours,
            Credits = dto.Credits,
            DistrictCost = dto.DistrictCost,
            EmployeeCost = dto.EmployeeCost
        };

        try 
        {
            await _courseService.CreateCourseAsync(course);
            return Ok(new { Message = "Course created successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message); // Handles the "No duplicate" rule
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null) return NotFound();
        return Ok(new CourseResponseDto(
            course.CourseId, course.Description, course.CourseType?.Description ?? "N/A",
            course.StartDate, course.EndDate, course.Hours, course.Approved));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdateCourseDto dto)
    {
        var course = new Course
        {
            CourseId = id,
            Description = dto.Description,
            CourseTypeId = dto.CourseTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Hours = dto.Hours,
            Credits = dto.Credits,
            DistrictCost = dto.DistrictCost,
            EmployeeCost = dto.EmployeeCost
        };
        try
        {
            await _courseService.UpdateCourseAsync(course);
            return Ok(new { Message = "Course updated successfully." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _courseService.DeleteCourseAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message); // Handles the "No delete if used" rule
        }
    }
}