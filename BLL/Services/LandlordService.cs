using AutoMapper;
using Microsoft.Extensions.Logging;
using BLL.DTOs.Landlord;
using BLL.Exceptions;
using BLL.Interfaces;
using Domain.Models;
using DAL.Interfaces;

namespace BLL.Services
{
    public class LandlordService : ILandlordService
    {
        private readonly ILandlordRepository _landlordRepo;
        private readonly IAccommodationRepository _accommodationRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<LandlordService> _logger;

        public LandlordService(
            ILandlordRepository landlordRepo,
            IAccommodationRepository accommodationRepo,
            IMapper mapper,
            ILogger<LandlordService> logger)
        {
            _landlordRepo = landlordRepo;
            _accommodationRepo = accommodationRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<LandlordDto> GetLandlordAsync(int id)
        {
            _logger.LogInformation("Fetching landlord with ID: {Id}", id);
            try
            {
                var landlord = await _landlordRepo.GetByIdWithPropertiesAsync(id);
                if (landlord == null)
                {
                    _logger.LogWarning("Landlord with ID {Id} not found", id);
                    throw new NotFoundException($"Landlord with ID {id} not found");
                }

                var dto = _mapper.Map<LandlordDto>(landlord);
                dto.ActiveListingsCount = landlord.Accommodations?.Count(a => a.IsAvailable) ?? 0;
                dto.TotalMonthlyRent = landlord.Accommodations?.Sum(a => a.MonthlyRent) ?? 0;

                _logger.LogInformation("Landlord with ID {Id} fetched successfully", id);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving landlord with ID {Id}", id);
                throw;
            }
        }

        public async Task<int> CreateAsync(LandlordRegistrationDto dto)
        {
            _logger.LogInformation("Creating new landlord with email: {Email}", dto.Email);
            var entity = _mapper.Map<Landlord>(dto);
            await _landlordRepo.AddAsync(entity);
            _logger.LogInformation("Landlord created with ID: {Id}", entity.LandlordId);
            return entity.LandlordId;
        }

        public async Task VerifyLandlordAsync(int landlordId)
        {
            _logger.LogInformation("Verifying landlord ID: {Id}", landlordId);
            try
            {
                var landlord = await _landlordRepo.GetByIdAsync(landlordId);
                if (landlord == null)
                {
                    _logger.LogWarning("Landlord with ID {Id} not found for verification", landlordId);
                    throw new NotFoundException($"Landlord with ID {landlordId} not found");
                }

                if (!string.IsNullOrEmpty(landlord.CompanyName) &&
                    string.IsNullOrEmpty(landlord.TaxIdentificationNumber))
                {
                    _logger.LogWarning("Corporate landlord missing tax ID: {Id}", landlordId);
                    throw new BusinessRuleException("Corporate landlords must provide tax ID");
                }

                landlord.IsVerified = true;
                landlord.VerificationDate = DateTime.UtcNow;
                await _landlordRepo.UpdateAsync(landlord);
                _logger.LogInformation("Landlord ID {Id} verified successfully", landlordId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying landlord with ID {Id}", landlordId);
                throw;
            }
        }

        public async Task<IEnumerable<LandlordDto>> GetLandlordsByUniversityAsync(int universityId)
        {
            _logger.LogInformation("Fetching landlords for university ID: {UniversityId}", universityId);
            try
            {
                var landlords = await _landlordRepo.GetByUniversityAsync(universityId);
                _logger.LogInformation("Fetched {Count} landlords for university ID: {UniversityId}", landlords.Count(), universityId);
                return _mapper.Map<IEnumerable<LandlordDto>>(landlords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving landlords for university ID {UniversityId}", universityId);
                throw;
            }
        }

        public async Task<IEnumerable<LandlordDto>> GetAllVerifiedLandlordsAsync()
        {
            _logger.LogInformation("Fetching all verified landlords");
            var landlords = await _landlordRepo.GetActiveLandlordsAsync();
            _logger.LogInformation("Fetched {Count} verified landlords", landlords.Count());
            return _mapper.Map<IEnumerable<LandlordDto>>(landlords);
        }

        public async Task UpdateLandlordProfileAsync(int landlordId, LandlordUpdateDto updateDto)
        {
            _logger.LogInformation("Updating profile for landlord ID: {Id}", landlordId);

            var landlord = await _landlordRepo.GetByIdAsync(landlordId);
            if (landlord == null)
            {
                _logger.LogWarning("Landlord with ID {Id} not found for profile update", landlordId);
                throw new NotFoundException($"Landlord with ID {landlordId} not found");
            }

            if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                landlord.FirstName = updateDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updateDto.MiddleName))
                landlord.MiddleName = updateDto.MiddleName;

            if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                landlord.LastName = updateDto.LastName;

            if (!string.IsNullOrWhiteSpace(updateDto.Email))
                landlord.Email = updateDto.Email;

            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
                landlord.PhoneNumber = updateDto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updateDto.CompanyName))
                landlord.CompanyName = updateDto.CompanyName;

            if (!string.IsNullOrWhiteSpace(updateDto.TaxIdentificationNumber))
                landlord.TaxIdentificationNumber = updateDto.TaxIdentificationNumber;

            landlord.UpdatedAt = DateTime.UtcNow;

            await _landlordRepo.UpdateAsync(landlord);
            _logger.LogInformation("Landlord ID {Id} profile updated", landlordId);
        }

        public async Task<IEnumerable<LandlordDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all landlords");
            var landlords = await _landlordRepo.GetAllAsync();
            _logger.LogInformation("Fetched {Count} landlords", landlords.Count());
            return _mapper.Map<IEnumerable<LandlordDto>>(landlords);
        }

        public async Task UpdateVerificationStatusAsync(int landlordId, LandlordVerificationDto dto)
        {
            _logger.LogInformation("Updating verification status for landlord ID: {Id}", landlordId);
            try
            {
                var landlord = await _landlordRepo.GetByIdAsync(landlordId);
                if (landlord == null)
                {
                    _logger.LogWarning("Landlord with ID {Id} not found for verification update", landlordId);
                    throw new NotFoundException($"Landlord with ID {landlordId} not found");
                }

                landlord.IsVerified = dto.IsVerified;
                landlord.VerificationDate = dto.VerificationDate;
                landlord.UpdatedAt = dto.UpdatedAt;

                await _landlordRepo.UpdateAsync(landlord);
                _logger.LogInformation("Landlord ID {Id} verification updated", landlordId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating verification for landlord ID {Id}", landlordId);
                throw;
            }
        }

        public async Task<LandlordBasicDto> GetPublicLandlordAsync(int id)
        {
            _logger.LogInformation("Fetching public profile for landlord ID: {Id}", id);

            var landlord = await _landlordRepo.GetByIdWithPropertiesAsync(id);
            if (landlord == null)
            {
                _logger.LogWarning("Landlord with ID {Id} not found for public profile", id);
                throw new NotFoundException("Landlord not found");
            }

            _logger.LogInformation("Public profile for landlord ID {Id} retrieved", id);
            return _mapper.Map<LandlordBasicDto>(landlord);
        }

        public async Task<LandlordAdminDto> GetLandlordForAdminAsync(int id)
        {
            _logger.LogInformation("Fetching admin view of landlord ID: {Id}", id);

            var landlord = await _landlordRepo.GetByIdWithPropertiesAsync(id);
            if (landlord == null)
            {
                _logger.LogWarning("Landlord with ID {Id} not found for admin view", id);
                throw new NotFoundException("Landlord not found");
            }

            _logger.LogInformation("Admin view for landlord ID {Id} retrieved", id);
            return _mapper.Map<LandlordAdminDto>(landlord);
        }

        public async Task<LandlordDto?> GetByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching landlord by user ID: {UserId}", userId);

            var landlord = await _landlordRepo.GetByUserIdAsync(userId);
            if (landlord == null)
            {
                _logger.LogWarning("Landlord not found for user ID: {UserId}", userId);
                return null;
            }

            var dto = _mapper.Map<LandlordDto>(landlord);
            dto.ActiveListingsCount = landlord.Accommodations?.Count(a => a.IsAvailable) ?? 0;
            dto.TotalMonthlyRent = landlord.Accommodations?.Sum(a => a.MonthlyRent) ?? 0;

            _logger.LogInformation("Landlord profile for user ID {UserId} retrieved", userId);
            return dto;
        }
    }
}
