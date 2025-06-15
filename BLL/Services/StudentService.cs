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
            _logger.LogInformation("Fetching student by ID: {StudentId}", id);

            try
            {
                var student = await _studentRepo.GetByIdAsync(id);
                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found", id);
                    throw new NotFoundException($"Student {id} not found");
                }

                _logger.LogInformation("Student with ID {StudentId} retrieved successfully", id);
                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
                throw;
            }
        }

        public async Task<int> CreateAsync(StudentRegistrationDto dto)
        {
            _logger.LogInformation("Creating new student with email: {Email}", dto.Email);

            try
            {
                var entity = _mapper.Map<Student>(dto);
                await _studentRepo.AddAsync(entity);
                _logger.LogInformation("Student created with ID: {StudentId}", entity.StudentId);
                return entity.StudentId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student with email: {Email}", dto.Email);
                throw;
            }
        }

        public async Task UpdateAsync(int studentId, StudentUpdateDto dto)
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", studentId);

            try
            {
                var student = await _studentRepo.GetByIdAsync(studentId);
                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found for update", studentId);
                    throw new NotFoundException($"Student {studentId} not found");
                }

                _mapper.Map(dto, student);
                await _studentRepo.UpdateAsync(student);
                _logger.LogInformation("Student with ID {StudentId} updated successfully", studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student with ID {StudentId}", studentId);
                throw;
            }
        }

        public async Task<StudentDto?> GetByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching student by user ID: {UserId}", userId);

            try
            {
                var student = await _studentRepo.GetByUserIdAsync(userId);

                if (student == null)
                {
                    _logger.LogWarning("No student found for user ID: {UserId}", userId);
                    return null;
                }

                _logger.LogInformation("Student for user ID {UserId} retrieved successfully", userId);
                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student by user ID {UserId}", userId);
                throw;
            }
        }
    }
}
