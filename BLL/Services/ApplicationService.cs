using AutoMapper;
using BLL.DTOs.Application;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(IApplicationRepository applicationRepo, IMapper mapper, ILogger<ApplicationService> logger)
        {
            _applicationRepo = applicationRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ApplicationDto>> GetByStudentAsync(int studentId)
        {
            _logger.LogInformation("Fetching applications for student ID: {StudentId}", studentId);

            var apps = await _applicationRepo.GetByStudentAsync(studentId);

            _logger.LogInformation("Retrieved {Count} applications for student ID: {StudentId}", apps.Count(), studentId);
            return _mapper.Map<IEnumerable<ApplicationDto>>(apps);
        }

        public async Task<IEnumerable<ApplicationDto>> GetByLandlordAsync(int landlordId)
        {
            _logger.LogInformation("Fetching applications for landlord ID: {LandlordId}", landlordId);

            var apps = await _applicationRepo.GetByLandlordIdAsync(landlordId);

            _logger.LogInformation("Retrieved {Count} applications for landlord ID: {LandlordId}", apps.Count(), landlordId);
            return _mapper.Map<IEnumerable<ApplicationDto>>(apps);
        }

        public async Task<ApplicationDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching application with ID: {Id}", id);

            var app = await _applicationRepo.GetByIdAsync(id);
            if (app == null)
            {
                _logger.LogWarning("Application with ID {Id} not found", id);
                throw new NotFoundException($"Application {id} not found");
            }

            _logger.LogInformation("Application with ID {Id} found", id);
            return _mapper.Map<ApplicationDto>(app);
        }

        public async Task<int> CreateAsync(ApplicationCreateDto dto)
        {
            _logger.LogInformation("Creating new application for student ID {StudentId} and accommodation ID {AccommodationId}", dto.StudentId, dto.AccommodationId);

            var entity = _mapper.Map<Application>(dto);
            var applicationId = await _applicationRepo.CreateAsync(entity);

            _logger.LogInformation("Application created with ID: {ApplicationId}", applicationId);
            return applicationId;
        }

        public async Task UpdateStatusAsync(ApplicationUpdateDto dto)
        {
            _logger.LogInformation("Updating status of application ID: {ApplicationId} to status ID: {StatusId}", dto.ApplicationId, dto.StatusId);

            var app = await _applicationRepo.GetByIdAsync(dto.ApplicationId);
            if (app == null)
            {
                _logger.LogWarning("Application with ID {Id} not found for update", dto.ApplicationId);
                throw new NotFoundException($"Application {dto.ApplicationId} not found");
            }

            app.StatusId = dto.StatusId;
            await _applicationRepo.UpdateAsync(app);

            _logger.LogInformation("Application ID {ApplicationId} status updated successfully", dto.ApplicationId);
        }

        public async Task<bool> ExistsAsync(int studentId, int accommodationId)
        {
            _logger.LogInformation("Checking if application exists for student ID {StudentId} and accommodation ID {AccommodationId}", studentId, accommodationId);

            var exists = await _applicationRepo.ExistsAsync(studentId, accommodationId);

            _logger.LogInformation("Application exists: {Exists}", exists);
            return exists;
        }

        public async Task<List<ApplicationDto>> GetByAccommodationIdAsync(int accommodationId)
        {
            _logger.LogInformation("Fetching applications for accommodation ID: {AccommodationId}", accommodationId);

            var apps = await _applicationRepo.GetByAccommodationIdAsync(accommodationId);

            _logger.LogInformation("Retrieved {Count} applications for accommodation ID: {AccommodationId}", apps.Count, accommodationId);

            return apps.Select(a => new ApplicationDto
            {
                ApplicationId = a.ApplicationId,
                StudentId = a.StudentId,
                AccommodationId = a.AccommodationId,
                StatusId = a.StatusId
            }).ToList();
        }

        public async Task SelectApplicantAsync(int applicationId, int accommodationId)
        {
            _logger.LogInformation("Selecting application ID {ApplicationId} and rejecting others for accommodation ID {AccommodationId}", applicationId, accommodationId);

            await _applicationRepo.MarkAsSelectedAsync(applicationId);
            await _applicationRepo.RejectOthersAsync(accommodationId, applicationId);

            _logger.LogInformation("Applicant selected and others rejected for accommodation ID: {AccommodationId}", accommodationId);
        }
    }
}
