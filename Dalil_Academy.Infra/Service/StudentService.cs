using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.DTOs.Students;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Mappings;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Core.Service;
using FluentValidation;

namespace Dalil_Academy.Infra.Service;

public class StudentService(
    IStudentRepository studentRepository,
    IValidator<CreateStudentDto> createValidator,
    IValidator<UpdateStudentDto> updateValidator)
    : IStudentService
{
    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await studentRepository.GetAllAsync();
        return students.Select(s => s.ToDto()).ToList();
    }

    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await studentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");  // 404

        return student.ToDto();
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);                    //  400

        var student = dto.ToEntity();
        var created = await studentRepository.CreateAsync(student);         // 409

        return created.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateStudentDto dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);                    //  400

        var student = await studentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");  //  404

        student.UpdateFromDto(dto);

        await studentRepository.UpdateAsync(student);                       //  409
    }

    public async Task DeleteAsync(int id)
    {
        if (!await studentRepository.DeleteAsync(id))
            throw new NotFoundException($"Student {id} was not found.");     //  404
    }

    
    public async Task<List<CourseResponseDto>> GetCoursesAsync(int id)
    {
        var student = await studentRepository.GetWithCoursesAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");  //  404

        return student.Enrollments
            .Where(e => e.Course is not null)
            .Select(e => e.Course!.ToDto())
            .ToList();
    }

    public async Task<PageResult<StudentResponseDto>> GetPageAsync(
    int page,
    int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var pagedResult = await studentRepository.GetPageAsync(
            page,
            pageSize);

        var dtos = pagedResult.Items
            .Select(s => s.ToDto())
            .ToList();

        return new PageResult<StudentResponseDto>(
            dtos,
            page,
            pageSize,
            pagedResult.TotalCount
        );
    }
}
