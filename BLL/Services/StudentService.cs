using AutoMapper;
using Microsoft.Extensions.Logging;
using BLL.Exceptions;
using BLL.Interfaces;
using Domain.Models;
using DAL.Interfaces;
using BLL.DTOs.Student;

namespace BLL.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IStudentRepository studentRepo,
            IMapper mapper,
            ILogger<StudentService> logger)
        {
            _studentRepo = studentRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StudentDto> GetByIdAsync(int id)
        {
            try
            {
                var student = await _studentRepo.GetByIdAsync(id);
                if (student == null)
                    throw new NotFoundException($"Student {id} not found");

                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving student with ID {id}");
                throw;
            }
        }

        public async Task<int> CreateAsync(StudentRegistrationDto dto)
        {
            try
            {
                var entity = _mapper.Map<Student>(dto);
                await _studentRepo.AddAsync(entity);
                return entity.StudentId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                throw;
            }
        }

        public async Task UpdateAsync(int studentId, StudentUpdateDto dto)
        {
            try
            {
                var student = await _studentRepo.GetByIdAsync(studentId);
                if (student == null)
                    throw new NotFoundException($"Student {studentId} not found");

                _mapper.Map(dto, student);
                await _studentRepo.UpdateAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating student with ID {studentId}");
                throw;
            }
        }

        public async Task<StudentDto?> GetByUserIdAsync(string userId)
        {
            try
            {
                var student = await _studentRepo.GetByUserIdAsync(userId);
                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving student by UserId {userId}");
                throw;
            }
        }
    }
}
