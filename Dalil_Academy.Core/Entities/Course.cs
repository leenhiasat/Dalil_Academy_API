using System;
using System.Collections.Generic;
using System.Text;

namespace Dalil_Academy.Core.Entities
{
   

    public class Course
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int InstructorId { get; set; }

        public Instructor? Instructor { get; set; }

        public List<Enrollment> Enrollments { get; set; } = [];

        public string Level { get; set; } = string.Empty;
        public int? MaxStudents { get; set; }

    }
}
