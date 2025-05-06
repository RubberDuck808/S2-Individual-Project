using AutoMapper;
using Microsoft.Extensions.Logging;
using UniNest.BLL.DTOs.Landlord;
using UniNest.BLL.Exceptions;
using UniNest.BLL.Interfaces;
using UniNest.DAL.Entities;
using UniNest.DAL.Interfaces;

namespace UniNest.BLL.Services
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

            _mapper.Map(updateDto, landlord);
            await _landlordRepo.UpdateAsync(landlord);
        }

        public async Task<IEnumerable<LandlordDto>> GetAllAsync()
        {
            var landlords = await _landlordRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<LandlordDto>>(landlords);
        }

    }
}
