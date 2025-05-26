using AutoMapper;
using BLL.DTOs.Accommodation;
using DAL.Models;
using BLL.Exceptions;
using DAL.Interfaces;
using BLL.Interfaces;

namespace BLL.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly IAccommodationRepository _accommodationRepo;
        private readonly IMapper _mapper;

        public AccommodationService(IAccommodationRepository accommodationRepo, IMapper mapper)
        {
            _accommodationRepo = accommodationRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccommodationDto>> GetAllAsync()
        {
            var entities = await _accommodationRepo.GetAvailableAccommodationsAsync();
            return _mapper.Map<IEnumerable<AccommodationDto>>(entities);
        }

        public async Task<AccommodationDto> GetByIdAsync(int id)
        {
            var accommodation = await _accommodationRepo.GetAccommodationWithDetailsAsync(id);
            if (accommodation == null)
                throw new NotFoundException($"Accommodation {id} not found");

            return _mapper.Map<AccommodationDto>(accommodation);
        }

        public async Task<int> CreateAsync(AccommodationCreateDto dto)
        {
            var entity = _mapper.Map<Accommodation>(dto);
            await _accommodationRepo.AddAsync(entity);
            return entity.AccommodationId;
        }

        public async Task UpdateAsync(AccommodationUpdateDto dto)
        {
            var accommodation = await _accommodationRepo.GetByIdAsync(dto.AccommodationId);
            if (accommodation == null)
                throw new NotFoundException($"Accommodation {dto.AccommodationId} not found");

            _mapper.Map(dto, accommodation);
            await _accommodationRepo.UpdateAsync(accommodation);
        }

        public async Task DeleteAsync(int id)
        {
            await _accommodationRepo.DeleteAsync(id);
        }
    }
}
