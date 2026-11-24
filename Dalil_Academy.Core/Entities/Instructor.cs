using Dalil_Academy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dalil_Academy.Core.Entities
{
    public class Instructor
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public List<Course> Courses { get; set; } = [];
    }
}
