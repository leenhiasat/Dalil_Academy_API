using Dalil_Academy.Core.DTOs.Students;
using Dalil_Academy.Core.Entities;
using global::Dalil_Academy.Core.DTOs.Students;
using global::Dalil_Academy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
namespace Dalil_Academy.Core.Mappings
{
    public static class StudentMapping
    {
        
        public static StudentResponseDto ToDto(this Student student)
        {
            return new StudentResponseDto(
                student.Id,
                student.FullName,
                student.Email
            );
        }

        public static Student ToEntity(this CreateStudentDto dto)
        {
            return new Student
            {
                FullName = dto.FullName,
                Email = dto.Email
            };
        }

        public static void UpdateFromDto(
            this Student student,
            UpdateStudentDto dto)
        {
            student.FullName = dto.FullName;
            student.Email = dto.Email;
        }
    }

}
