using AutoMapper;
using Microsoft.Extensions.Logging;
using BLL.DTOs.Landlord;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Models;
using DAL.Interfaces;
using BLL.DTOs.Student;

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
            try
            {
                var landlord = await _landlordRepo.GetByIdWithPropertiesAsync(id);
                if (landlord == null)
                    throw new NotFoundException($"Landlord with ID {id} not found");

                var dto = _mapper.Map<LandlordDto>(landlord);
                dto.ActiveListingsCount = landlord.Accommodations?.Count(a => a.IsAvailable) ?? 0;
                dto.TotalMonthlyRent = landlord.Accommodations?.Sum(a => a.MonthlyRent) ?? 0;

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving landlord with ID {id}");
                throw;
            }
        }


        public async Task<int> CreateAsync(LandlordRegistrationDto dto)
        {
            var entity = _mapper.Map<Landlord>(dto);
            await _landlordRepo.AddAsync(entity);
            return entity.LandlordId;
        }
        public async Task VerifyLandlordAsync(int landlordId)
        {
            try
            {
                var landlord = await _landlordRepo.GetByIdAsync(landlordId);
                if (landlord == null)
                    throw new NotFoundException($"Landlord with ID {landlordId} not found");

                if (!string.IsNullOrEmpty(landlord.CompanyName) &&
                    string.IsNullOrEmpty(landlord.TaxIdentificationNumber))
                {
                    throw new BusinessRuleException("Corporate landlords must provide tax ID");
                }

                landlord.IsVerified = true;
                landlord.VerificationDate = DateTime.UtcNow;
                await _landlordRepo.UpdateAsync(landlord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying landlord with ID {landlordId}");
                throw;
            }
        }

        public async Task<IEnumerable<LandlordDto>> GetLandlordsByUniversityAsync(int universityId)
        {
            try
            {
                var landlords = await _landlordRepo.GetByUniversityAsync(universityId);
                return _mapper.Map<IEnumerable<LandlordDto>>(landlords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving landlords for university {universityId}");
                throw;
            }
        }

        public async Task<IEnumerable<LandlordDto>> GetAllVerifiedLandlordsAsync()
        {
            var landlords = await _landlordRepo.GetActiveLandlordsAsync();
            return _mapper.Map<IEnumerable<LandlordDto>>(landlords);
        }

        public async Task UpdateLandlordProfileAsync(int landlordId, LandlordUpdateDto updateDto)
        {
            var landlord = await _landlordRepo.GetByIdAsync(landlordId);
            if (landlord == null)
                throw new NotFoundException($"Landlord with ID {landlordId} not found");

            // Only update if the field has a new value
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
        }


        public async Task<IEnumerable<LandlordDto>> GetAllAsync()
        {
            var landlords = await _landlordRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<LandlordDto>>(landlords);
        }

        public async Task UpdateVerificationStatusAsync(int landlordId, LandlordVerificationDto dto)
        {
            try
            {
                var landlord = await _landlordRepo.GetByIdAsync(landlordId);
                if (landlord == null)
                    throw new NotFoundException($"Landlord with ID {landlordId} not found");

                landlord.IsVerified = dto.IsVerified;
                landlord.VerificationDate = dto.VerificationDate;
                landlord.UpdatedAt = dto.UpdatedAt;

                await _landlordRepo.UpdateAsync(landlord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating verification for landlord ID {landlordId}");
                throw;
            }
        }

        public async Task<LandlordBasicDto> GetPublicLandlordAsync(int id)
        {
            var landlord = await _landlordRepo.GetByIdWithPropertiesAsync(id);
            if (landlord == null)
                throw new NotFoundException("Landlord not found");

            return _mapper.Map<LandlordBasicDto>(landlord);
        }

        public async Task<LandlordAdminDto> GetLandlordForAdminAsync(int id)
        {
            var landlord = await _landlordRepo.GetByIdWithPropertiesAsync(id);
            if (landlord == null)
                throw new NotFoundException("Landlord not found");

            return _mapper.Map<LandlordAdminDto>(landlord);
        }

        public async Task<LandlordDto?> GetByUserIdAsync(string userId)
        {
            var landlord = await _landlordRepo.GetByUserIdAsync(userId);
            if (landlord == null) return null;

            var dto = _mapper.Map<LandlordDto>(landlord);
            dto.ActiveListingsCount = landlord.Accommodations?.Count(a => a.IsAvailable) ?? 0;
            dto.TotalMonthlyRent = landlord.Accommodations?.Sum(a => a.MonthlyRent) ?? 0;

            return dto;
        }



    }
}
