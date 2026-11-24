using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Service;
using Dalil_Academy.Infra.Data;
using Dalil_Academy.Infra.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dalil_Academy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)] 
public class StudentsController(AppDbContext db, IStudentService studentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllStudents(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        var students = await studentService.GetPageAsync(
            page,
            pageSize);

        return Ok(students);
    }



    [HttpGet("{id}/courses")]
    public async Task<IActionResult> GetStudentCourses(int id)
    {
        var courses = await studentService.GetCoursesAsync(id);
        return Ok(courses);
    }
}