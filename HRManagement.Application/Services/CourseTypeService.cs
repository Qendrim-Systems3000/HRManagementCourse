using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Services;

public class CourseTypeService : ICourseTypeService
{
    private readonly ICourseTypeRepository _repository;

    public CourseTypeService(ICourseTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CourseType>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<CourseType?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task CreateAsync(CourseType courseType)
    {
        if (await _repository.ExistsByDescriptionAsync(courseType.Description))
            throw new InvalidOperationException("A course type with this description already exists.");

        await _repository.AddAsync(courseType);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(CourseType courseType)
    {
        var existing = await _repository.GetByIdAsync(courseType.CourseTypeId);
        if (existing == null) throw new KeyNotFoundException("Course type not found.");

        if (existing.Description != courseType.Description && await _repository.ExistsByDescriptionAsync(courseType.Description, courseType.CourseTypeId))
            throw new InvalidOperationException("A course type with this description already exists.");

        existing.Description = courseType.Description;
        _repository.Update(existing);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (await _repository.IsUsedInCoursesAsync(id))
            throw new InvalidOperationException("Cannot delete course type: it is used by one or more courses.");

        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) throw new KeyNotFoundException("Course type not found.");

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }
}
