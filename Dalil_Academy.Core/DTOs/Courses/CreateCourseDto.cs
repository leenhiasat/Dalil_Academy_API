using System;
using System.Collections.Generic;
using System.Text;

namespace Dalil_Academy.Core.DTOs.Courses
{

    public record CreateCourseDto(string Title, decimal Price, int InstructorId, int? MaxStudents=null);

}

