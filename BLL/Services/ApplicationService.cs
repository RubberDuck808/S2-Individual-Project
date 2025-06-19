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
                throw new NotFoundException(string.Format(ErrorMessages.AccommodationNotFound, id));
            }

            _logger.LogInformation("Application with ID {Id} found", id);
            return _mapper.Map<ApplicationDto>(app);
        }

        public async Task<int> CreateAsync(ApplicationCreateDto dto)
        {
            if (await _applicationRepo.ExistsAsync(dto.StudentId, dto.AccommodationId))
                throw new ValidationException("You have already applied to this accommodation.");

            //var accommodation = await _accommodationService.GetEntityAsync(dto.AccommodationId);
            //if (accommodation == null)
            //    throw new NotFoundException($"Accommodation {dto.AccommodationId} not found.");

            //if (!accommodation.IsAvailable)
            //    throw new ValidationException("This accommodation is no longer available for application.");



            var application = new Application
            {
                StudentId = dto.StudentId,
                AccommodationId = dto.AccommodationId,
                StatusId = 1, // e.g. Pending
                ApplicationDate = DateTime.UtcNow
            };

            return await _applicationRepo.CreateAsync(application);
        }

        public async Task<bool> ExistsAsync(int studentId, int accommodationId)
        {
            _logger.LogInformation("Checking if application exists for student ID {StudentId} and accommodation ID {AccommodationId}", studentId, accommodationId);

            var exists = await _applicationRepo.ExistsAsync(studentId, accommodationId);

            _logger.LogInformation("Application exists: {Exists}", exists);
            return exists;
        }


        public async Task UpdateStatusAsync(ApplicationUpdateDto dto)
        {
            _logger.LogInformation("Updating status of application ID: {ApplicationId} to status ID: {StatusId}", dto.ApplicationId, dto.StatusId);

            var app = await _applicationRepo.GetByIdAsync(dto.ApplicationId);
            if (app == null)
            {
                _logger.LogWarning("Application with ID {Id} not found for update", dto.ApplicationId);
                throw new NotFoundException(string.Format(ErrorMessages.ApplicationNotFound, dto.ApplicationId));

            }

            app.StatusId = dto.StatusId;
            await _applicationRepo.UpdateAsync(app);

            _logger.LogInformation("Application ID {ApplicationId} status updated successfully", dto.ApplicationId);
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

        public async Task<IEnumerable<(int AccommodationId, int Count)>> GetApplicationCountsByLandlordIdAsync(int landlordId)
        {
            var dict = await _applicationRepo.GetApplicationCountsByLandlordIdAsync(landlordId);
            return dict.Select(entry => (AccommodationId: entry.Key, Count: entry.Value));
        }

        public async Task<List<(int ApplicationId, int AccommodationId)>> GetApplicationsWithAccommodationIdsByStudentAsync(int studentId)
        {
            _logger.LogInformation("Fetching application IDs and accommodation IDs for student ID: {StudentId}", studentId);
            return await _applicationRepo.GetApplicationsWithAccommodationIdsByStudentAsync(studentId);
        }

        public async Task<string?> GetStatusNameByStudentAndAccommodationIdAsync(int studentId, int accommodationId)
        {
            _logger.LogInformation("Fetching status name for student ID {StudentId} and accommodation ID {AccommodationId}", studentId, accommodationId);
            return await _applicationRepo.GetStatusNameByStudentAndAccommodationIdAsync(studentId, accommodationId);
        }


    }
}
