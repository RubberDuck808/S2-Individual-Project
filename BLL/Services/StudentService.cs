using AutoMapper;
using BLL.DTOs.Student;
using DAL.Models;
using BLL.Exceptions;
using DAL.Interfaces;
using BLL.Interfaces;

namespace BLL.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepo, IMapper mapper)
        {
            _studentRepo = studentRepo;
            _mapper = mapper;
        }

        public async Task<StudentDto> GetByIdAsync(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException($"Student {id} not found");

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<int> CreateAsync(StudentRegistrationDto dto)
        {
            var entity = _mapper.Map<Student>(dto);
            await _studentRepo.AddAsync(entity);
            return entity.StudentId;
        }

        public async Task UpdateAsync(int studentId, StudentUpdateDto dto)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null)
                throw new NotFoundException($"Student {studentId} not found");

            _mapper.Map(dto, student);
            await _studentRepo.UpdateAsync(student);
        }
    }
}
