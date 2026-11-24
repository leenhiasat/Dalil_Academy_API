
using Dalil_Academy.Core.DTOs.Enrollments;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dalil_Academy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController(
    IEnrollmentService enrollmentService,
    IPaymentService paymentService) : ControllerBase
{
    // GET: api/Enrollments/me
    [HttpGet("me")]
    [Authorize(Roles = Roles.Student)]
    public async Task<IActionResult> GetMyEnrolledCourses()
    {
        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) ||
            !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var courses =
            await enrollmentService.GetMyCoursesAsync(userId);

        return Ok(courses);
    }

    // PUT: api/Enrollments/{id}?courseId=2
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateEnrollment(
        int id,
        [FromQuery] int courseId)
    {
        var result =
            await enrollmentService.UpdateEnrollmentAsync(id, courseId);

        return Ok(result);
    }

    // DELETE: api/Enrollments/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        await enrollmentService.DeleteEnrollmentAsync(id);

        return NoContent();
    }

    // POST: api/Enrollments/{id}/pay
    [HttpPost("{id}/pay")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Pay(int id)
    {
        var result = await paymentService.PayAsync(id);

        return Ok(result);
    }

    // POST: api/Enrollments/{id}/capture
    [HttpPost("{id}/capture")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Capture(int id)
    {
        var result = await paymentService.CaptureAsync(id);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Enroll(CreateEnrollmentDto dto)
    {
        var result = await enrollmentService.EnrollAsync(dto.StudentId, dto.CourseId);
        return Ok(result);
    }

    [HttpGet("{id}/payments")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetPaymentHistory(int id)
    {
        var payments =
            await paymentService.GetPaymentHistoryAsync(id);

        return Ok(payments);
    }
}
