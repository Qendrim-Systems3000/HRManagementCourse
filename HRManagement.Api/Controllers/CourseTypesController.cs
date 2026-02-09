using AutoMapper;
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
    private readonly IMapper _mapper;

    public CourseTypesController(ICourseTypeService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var list = await _service.GetAllAsync();
        var response = _mapper.Map<IEnumerable<CourseTypeResponseDto>>(list);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ct = await _service.GetByIdAsync(id);
        if (ct == null) return NotFound();
        return Ok(_mapper.Map<CourseTypeResponseDto>(ct));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRUser")]
    public async Task<IActionResult> Add([FromBody] CreateCourseTypeDto dto)
    {
        var entity = _mapper.Map<CourseType>(dto);
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
        var entity = _mapper.Map<CourseType>(dto);
        entity.CourseTypeId = id;
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
