using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dalil_Academy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(
    ICourseService courseService,
    IEnrollmentService enrollmentService)
    : ControllerBase
{
    [HttpGet("filter")]
    public async Task<IActionResult> GetByPriceRange(
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        var courses = await courseService.GetByPriceRangeAsync(
            minPrice,
            maxPrice);

        return Ok(courses);
    }

    [HttpGet("pages")]
    public async Task<ActionResult<PageResult<CourseResponseDto>>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        return Ok(
            await courseService.GetPageAsync(page, pageSize));
    }

    // Keeps the existing route: GET api/courses
    // Without query parameters, returns all courses.
    // With query parameters, performs search and pagination.
    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string? search,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        if (string.IsNullOrWhiteSpace(search) &&
            !page.HasValue &&
            !pageSize.HasValue)
        {
            return Ok(await courseService.GetAllAsync());
        }

        var result = await courseService.SearchAsync(
            search,
            page ?? 1,
            pageSize ?? 10);

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<CourseResponseDto>>> GetAll()
    {
        return Ok(await courseService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseResponseDto>> GetById(int id)
    {
        return Ok(await courseService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponseDto>> Create(
        CreateCourseDto dto)
    {
        var created = await courseService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCourseDto dto)
    {
        await courseService.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await courseService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCoursesCount()
    {
        var count = await courseService.GetCountAsync();

        return Ok(new
        {
            TotalCourses = count
        });
    }

    [HttpGet("{courseId:int}/students")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetCourseStudents(int courseId)
    {
        var students = await enrollmentService
            .GetPaidStudentsByCourseIdAsync(courseId);

        return Ok(students);
    }
}