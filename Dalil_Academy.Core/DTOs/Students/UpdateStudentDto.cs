using System;
using System.Collections.Generic;
using System.Text;

namespace Dalil_Academy.Core.DTOs.Students
{


    public record UpdateStudentDto(
        string FullName,
        string Email
    );
}
