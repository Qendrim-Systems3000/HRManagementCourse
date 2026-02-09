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
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;

    public CoursesController(ICourseService courseService, IMapper mapper)
    {
        _courseService = courseService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? typeId, [FromQuery] DateTime? date, [FromQuery] bool? approved)
    {
        var courses = await _courseService.GetCoursesAsync(typeId, date, approved);
        var response = _mapper.Map<IEnumerable<CourseResponseDto>>(courses);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        var course = _mapper.Map<Course>(dto);
        try
        {
            await _courseService.CreateCourseAsync(course);
            return Ok(new { Message = "Course created successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null) return NotFound();
        return Ok(_mapper.Map<CourseResponseDto>(course));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdateCourseDto dto)
    {
        var course = _mapper.Map<Course>(dto);
        course.CourseId = id;
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