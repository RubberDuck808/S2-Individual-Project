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
        private readonly IAmenityRepository _amenityRepo;
        private readonly IAccommodationImageRepository _imageRepo;
        private readonly IUniversityRepository _universityRepo;
        private readonly IAccommodationTypeRepository _typeRepo;
        private readonly IAccommodationRepository _accommodationRepo;
        private readonly IMapper _mapper;


        public AccommodationService(
            IAccommodationRepository accommodationRepo,
            IMapper mapper,
            IAmenityRepository amenityRepo,
            IAccommodationImageRepository imageRepo,
            IUniversityRepository universityRepo,
            IAccommodationTypeRepository typeRepo)
        {
            _accommodationRepo = accommodationRepo;
            _mapper = mapper;
            _amenityRepo = amenityRepo;
            _imageRepo = imageRepo;
            _universityRepo = universityRepo;
            _typeRepo = typeRepo;
        }


        public async Task<AccommodationDto> GetByIdAsync(int id)
        {
            var entity = await _accommodationRepo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException($"Accommodation {id} not found");

            var amenities = await _amenityRepo.GetByAccommodationIdAsync(id);
            var images = await _imageRepo.GetByAccommodationIdAsync(id);
            var university = await _universityRepo.GetByIdAsync(entity.UniversityId);
            var type = await _typeRepo.GetByIdAsync(entity.AccommodationTypeId);

            return new AccommodationDto
            {
                AccommodationId = entity.AccommodationId,
                Title = entity.Title,
                Description = entity.Description,
                Address = entity.Address,
                MonthlyRent = entity.MonthlyRent,
                IsAvailable = entity.IsAvailable,
                MaxOccupants = entity.MaxOccupants,
                Size = (int)entity.Size,
                AvailableFrom = entity.AvailableFrom,
                AmenityNames = amenities.Select(a => a.Name).ToList(),
                ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                UniversityName = university?.Name ?? "",
                AccommodationType = type?.Name ?? "",
                LandlordName = ""
            };
        }


        public async Task<int> CreateAsync(AccommodationCreateDto dto)
        {
            var entity = _mapper.Map<Accommodation>(dto);
            int insertedId = await _accommodationRepo.AddAsync(entity);
            return insertedId;
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

        public async Task AddAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds)
        {
            await _accommodationRepo.AddAmenitiesAsync(accommodationId, amenityIds);
        }

        public async Task AddImagesAsync(IEnumerable<AccommodationImage> images)
        {
            await _accommodationRepo.AddImagesAsync(images);
        }

        public async Task<int> CreateAccommodationWithAmenitiesAsync(AccommodationCreateDto dto, IEnumerable<int> amenityIds)
        {
            var entity = _mapper.Map<Accommodation>(dto);
            int accommodationId = await _accommodationRepo.AddAsync(entity);

            if (accommodationId <= 0)
                throw new Exception("Accommodation insert failed or returned invalid ID.");

            await _accommodationRepo.AddAmenitiesAsync(accommodationId, amenityIds);
            return accommodationId;
        }


        public async Task<IEnumerable<AccommodationDto>> GetAllAsync()
        {
            var entities = await _accommodationRepo.GetAvailableAccommodationsAsync();
            var dtos = new List<AccommodationDto>();

            foreach (var entity in entities)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(entity.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(entity.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(entity.UniversityId);
                var type = await _typeRepo.GetByIdAsync(entity.AccommodationTypeId);

                var dto = new AccommodationDto
                {
                    AccommodationId = entity.AccommodationId,
                    Title = entity.Title,
                    Description = entity.Description,
                    Address = entity.Address,
                    MonthlyRent = entity.MonthlyRent,
                    IsAvailable = entity.IsAvailable,
                    MaxOccupants = entity.MaxOccupants,
                    Size = (int)entity.Size,
                    AvailableFrom = entity.AvailableFrom,
                    AmenityNames = amenities.Select(a => a.Name).ToList(),
                    ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                    UniversityName = university?.Name ?? string.Empty,
                    AccommodationType = type?.Name ?? string.Empty,
                    LandlordName = ""  
                };

                dtos.Add(dto);
            }

            return dtos;
        }

    }
}
