using System;
using System.Collections.Generic;
using System.Text;

namespace Dalil_Academy.Core.DTOs.Courses
{
    public record UpdateCourseDto(string Title, decimal Price, int? MaxStudents=null);

}
