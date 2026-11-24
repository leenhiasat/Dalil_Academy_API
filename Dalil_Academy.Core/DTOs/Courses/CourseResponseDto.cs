using System;
using System.Collections.Generic;
using System.Text;

namespace Dalil_Academy.Core.DTOs.Courses
{
    public record CourseResponseDto(
      int Id,
      string Title,
      decimal Price,
      string? InstructorName,
      int? MaxStudents);
  

}
