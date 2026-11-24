using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.DTOs.Common;
using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Mappings;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Core.Service;
using FluentValidation;

namespace Dalil_Academy.Infra.Service;

public class CourseService(
    ICourseRepository courseRepository,
    IValidator<CreateCourseDto> createValidator,
    IValidator<UpdateCourseDto> updateValidator)
    : ICourseService
{



   public async Task<PageResult<CourseResponseDto>> GetPageAsync(int page, int pageSize)
{
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var pagedResult = await courseRepository.GetPageAsync(page, pageSize);
    
    var dtos = pagedResult.Items.Select(c => c.ToDto()).ToList();

        return new PageResult<CourseResponseDto>(dtos, page, pageSize, pagedResult.TotalCount);
}
    public async Task<List<CourseResponseDto>> GetAllAsync()
    {
        var courses = await courseRepository.GetAllAsync();
        return courses.Select(c => c.ToDto()).ToList();
    }

    public async Task<CourseResponseDto> GetByIdAsync(int id)
    {
        var course = await courseRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Course {id} was not found.");   // → 404

        return course.ToDto();
    }

    public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);                    // → 400

        var course = dto.ToEntity();
        var created = await courseRepository.CreateAsync(course);           //  409

        return created.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateCourseDto dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);                    // → 400

        var course = await courseRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Course {id} was not found.");   // → 404

        course.UpdateFromDto(dto);

        await courseRepository.UpdateAsync(course);                         //  409
    }

    public async Task DeleteAsync(int id)
    {
        if (!await courseRepository.DeleteAsync(id))
            throw new NotFoundException($"Course {id} was not found.");      // → 404
    }

    public async Task<List<CourseResponseDto>> GetByPriceRangeAsync(
    decimal? minPrice,
    decimal? maxPrice)
    {
        if (minPrice.HasValue && maxPrice.HasValue &&
            minPrice.Value > maxPrice.Value)
        {
            throw new ArgumentException(
                "Minimum price cannot exceed maximum price.");
        }

        var courses = await courseRepository.GetByPriceRangeAsync(
            minPrice,
            maxPrice);

        return courses.Select(c => c.ToDto()).ToList();
    }

    public async Task<int> GetCountAsync()
    {
        return await courseRepository.GetCountAsync();
    }

    public async Task<PagedResponseDto<CourseResponseDto>> SearchAsync(
    string? search,
    int page,
    int pageSize)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

        var result = await courseRepository.SearchAsync(
            search,
            page,
            pageSize);

        var courses = result.Items
            .Select(c => c.ToDto())
            .ToList();

        var totalPages = (int)Math.Ceiling(
            result.TotalCount / (double)pageSize);

        return new PagedResponseDto<CourseResponseDto>(
            courses,
            page,
            pageSize,
            result.TotalCount,
            totalPages
        );
    }
}
