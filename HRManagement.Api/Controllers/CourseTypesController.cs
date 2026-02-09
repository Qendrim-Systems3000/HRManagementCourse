using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;

namespace HRManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CourseTypesController : ControllerBase
{
    private readonly ICourseTypeService _service;

    public CourseTypesController(ICourseTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var list = await _service.GetAllAsync();
        var response = list.Select(ct => new CourseTypeResponseDto(ct.CourseTypeId, ct.Description));
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ct = await _service.GetByIdAsync(id);
        if (ct == null) return NotFound();
        return Ok(new CourseTypeResponseDto(ct.CourseTypeId, ct.Description));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Add([FromBody] CreateCourseTypeDto dto)
    {
        var entity = new CourseType { Description = dto.Description };
        try
        {
            await _service.CreateAsync(entity);
            return Ok(new { Message = "Course type created successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdateCourseTypeDto dto)
    {
        var entity = new CourseType { CourseTypeId = id, Description = dto.Description };
        try
        {
            await _service.UpdateAsync(entity);
            return Ok(new { Message = "Course type updated successfully." });
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

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
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
}
