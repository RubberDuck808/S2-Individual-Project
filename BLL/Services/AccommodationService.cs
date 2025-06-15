using AutoMapper;
using BLL.DTOs.Accommodation;
using Domain.Models;
using BLL.Exceptions;
using DAL.Interfaces;
using BLL.Interfaces;
using Microsoft.Extensions.Logging;

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
        private readonly ILandlordRepository _landlordRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IApplicationRepository _applicationRepo;
        private readonly ILogger<AccommodationService> _logger;

        public AccommodationService(
            IAccommodationRepository accommodationRepo,
            IMapper mapper,
            IAmenityRepository amenityRepo,
            IAccommodationImageRepository imageRepo,
            IUniversityRepository universityRepo,
            IAccommodationTypeRepository typeRepo,
            ILandlordRepository landlordRepo,
            IStudentRepository studentRepo,
            IApplicationRepository applicationRepo,
            ILogger<AccommodationService> logger)
        {
            _accommodationRepo = accommodationRepo;
            _mapper = mapper;
            _amenityRepo = amenityRepo;
            _imageRepo = imageRepo;
            _universityRepo = universityRepo;
            _typeRepo = typeRepo;
            _landlordRepo = landlordRepo;
            _studentRepo = studentRepo;
            _applicationRepo = applicationRepo;
            _logger = logger;
        }

        public async Task<AccommodationDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching accommodation with ID: {Id}", id);

            var entity = await _accommodationRepo.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Accommodation with ID {Id} not found", id);
                throw new NotFoundException($"Accommodation {id} not found");
            }

            var amenities = await _amenityRepo.GetByAccommodationIdAsync(id);
            var images = await _imageRepo.GetByAccommodationIdAsync(id);
            var university = await _universityRepo.GetByIdAsync(entity.UniversityId);
            var type = await _typeRepo.GetByIdAsync(entity.AccommodationTypeId);

            _logger.LogInformation("Accommodation {Id} loaded with amenities and images", id);

            return new AccommodationDto
            {
                AccommodationId = entity.AccommodationId,
                Title = entity.Title,
                Description = entity.Description,
                Address = entity.Address,
                PostCode = entity.PostCode,
                City = entity.City,
                Country = entity.Country,
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
            _logger.LogInformation("Creating new accommodation titled: {Title}", dto.Title);

            var entity = _mapper.Map<Accommodation>(dto);
            int insertedId = await _accommodationRepo.AddAsync(entity);

            if (insertedId <= 0)
            {
                _logger.LogError("Accommodation creation failed. Insert returned invalid ID: {Id}", insertedId);
                throw new Exception("Accommodation insert failed — no valid ID returned.");
            }

            _logger.LogInformation("Accommodation created with ID: {Id}", insertedId);
            return insertedId;
        }


        public async Task UpdateAsync(AccommodationUpdateDto dto)
        {
            _logger.LogInformation("Updating accommodation ID: {Id}", dto.AccommodationId);

            var accommodation = await _accommodationRepo.GetByIdAsync(dto.AccommodationId);
            if (accommodation == null)
            {
                _logger.LogWarning("Accommodation with ID {Id} not found for update", dto.AccommodationId);
                throw new NotFoundException($"Accommodation {dto.AccommodationId} not found");
            }

            _mapper.Map(dto, accommodation);
            await _accommodationRepo.UpdateAsync(accommodation);

            _logger.LogInformation("Accommodation ID {Id} updated successfully", dto.AccommodationId);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting accommodation with ID: {Id}", id);

            int affected = await _accommodationRepo.DeleteAsync(id);

            if (affected == 0)
            {
                _logger.LogWarning("Accommodation with ID {Id} not found for deletion", id);
                throw new NotFoundException($"Accommodation with ID {id} not found.");
            }

            _logger.LogInformation("Accommodation with ID {Id} deleted successfully", id);
        }


        public async Task AddAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds)
        {
            _logger.LogInformation("Adding amenities to accommodation ID: {Id}", accommodationId);
            await _accommodationRepo.AddAmenitiesAsync(accommodationId, amenityIds);
        }

        public async Task AddImagesAsync(IEnumerable<AccommodationImage> images)
        {
            _logger.LogInformation("Adding {Count} images to accommodation", images.Count());
            await _accommodationRepo.AddImagesAsync(images);
        }

        public async Task<int> CreateAccommodationWithAmenitiesAsync(AccommodationCreateDto dto, IEnumerable<int> amenityIds)
        {
            _logger.LogInformation("Creating accommodation with amenities: {AmenityCount}", amenityIds.Count());

            var entity = _mapper.Map<Accommodation>(dto);
            int accommodationId = await _accommodationRepo.AddAsync(entity);

            if (accommodationId <= 0)
            {
                _logger.LogError("Failed to create accommodation. Invalid ID returned.");
                throw new Exception("Accommodation insert failed or returned invalid ID.");
            }

            await _accommodationRepo.AddAmenitiesAsync(accommodationId, amenityIds);

            _logger.LogInformation("Accommodation with ID {Id} created and amenities added", accommodationId);
            return accommodationId;
        }

        public async Task<int> UpdateWithAmenitiesAsync(AccommodationUpdateDto dto, IEnumerable<int> amenityIds)
        {
            _logger.LogInformation("Updating accommodation with ID {Id} and amenities", dto.AccommodationId);

            var entity = _mapper.Map<Accommodation>(dto);
            await _accommodationRepo.UpdateAsync(entity);

            await _accommodationRepo.UpdateAmenitiesAsync(dto.AccommodationId, amenityIds);

            _logger.LogInformation("Accommodation ID {Id} and its amenities updated", dto.AccommodationId);
            return dto.AccommodationId;
        }

        public async Task<IEnumerable<AccommodationDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all available accommodations");

            var entities = await _accommodationRepo.GetAvailableAccommodationsAsync();
            var dtos = new List<AccommodationDto>();

            foreach (var entity in entities)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(entity.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(entity.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(entity.UniversityId);
                var type = await _typeRepo.GetByIdAsync(entity.AccommodationTypeId);

                dtos.Add(new AccommodationDto
                {
                    AccommodationId = entity.AccommodationId,
                    Title = entity.Title,
                    Description = entity.Description,
                    Address = entity.Address,
                    PostCode = entity.PostCode,
                    City = entity.City,
                    Country = entity.Country,
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
                });
            }

            _logger.LogInformation("Fetched {Count} accommodations", dtos.Count);
            return dtos;
        }

        public async Task<IEnumerable<AccommodationDto>> GetIndexAsync()
        {
            _logger.LogInformation("Fetching featured accommodations for index");

            var entities = await _accommodationRepo.GetFeaturedAccommodationsAsync();
            var dtos = new List<AccommodationDto>();

            foreach (var entity in entities)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(entity.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(entity.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(entity.UniversityId);
                var type = await _typeRepo.GetByIdAsync(entity.AccommodationTypeId);

                dtos.Add(new AccommodationDto
                {
                    AccommodationId = entity.AccommodationId,
                    Title = entity.Title,
                    Description = entity.Description,
                    Address = entity.Address,
                    PostCode = entity.PostCode,
                    City = entity.City,
                    Country = entity.Country,
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
                });
            }

            return dtos;
        }

        public async Task<IEnumerable<LandlordAccommodationDto>> GetByLandlordUserIdAsync(string landlordUserId)
        {
            _logger.LogInformation("Fetching listings for landlord user ID: {UserId}", landlordUserId);

            var landlord = await _landlordRepo.GetByUserIdAsync(landlordUserId);
            var listings = await _accommodationRepo.GetWithApplicationCountsByLandlordIdAsync(landlord.LandlordId);
            var result = new List<LandlordAccommodationDto>();

            foreach ((var accommodation, var count) in listings)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(accommodation.UniversityId);
                var type = await _typeRepo.GetByIdAsync(accommodation.AccommodationTypeId);

                result.Add(new LandlordAccommodationDto
                {
                    AccommodationId = accommodation.AccommodationId,
                    Title = accommodation.Title,
                    Description = accommodation.Description,
                    Address = accommodation.Address,
                    PostCode = accommodation.PostCode,
                    City = accommodation.City,
                    Country = accommodation.Country,
                    MonthlyRent = accommodation.MonthlyRent,
                    IsAvailable = accommodation.IsAvailable,
                    MaxOccupants = accommodation.MaxOccupants,
                    Size = (int)accommodation.Size,
                    AvailableFrom = accommodation.AvailableFrom,
                    ApplicationCount = count,
                    AmenityNames = amenities.Select(a => a.Name).ToList(),
                    ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                    UniversityName = university?.Name ?? string.Empty,
                    AccommodationType = type?.Name ?? string.Empty
                });
            }

            return result;
        }

        public async Task<IEnumerable<AppliedAccommodationDto>> GetByStudentUserIdAsync(string studentUserId)
        {
            _logger.LogInformation("Fetching applied accommodations for student user ID: {UserId}", studentUserId);

            var student = await _studentRepo.GetByUserIdAsync(studentUserId);
            var listings = await _accommodationRepo.GetWithApplicationsByStudentIdAsync(student.StudentId);
            var result = new List<AppliedAccommodationDto>();

            foreach (var accommodation in listings)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(accommodation.UniversityId);
                var type = await _typeRepo.GetByIdAsync(accommodation.AccommodationTypeId);
                var status = await _applicationRepo.GetStatusNameByStudentAndAccommodationIdAsync(student.StudentId, accommodation.AccommodationId);

                result.Add(new AppliedAccommodationDto
                {
                    AccommodationId = accommodation.AccommodationId,
                    Title = accommodation.Title,
                    Description = accommodation.Description,
                    Address = accommodation.Address,
                    PostCode = accommodation.PostCode,
                    City = accommodation.City,
                    Country = accommodation.Country,
                    MonthlyRent = accommodation.MonthlyRent,
                    IsAvailable = accommodation.IsAvailable,
                    MaxOccupants = accommodation.MaxOccupants,
                    Size = (int)accommodation.Size,
                    AvailableFrom = accommodation.AvailableFrom,
                    AmenityNames = amenities.Select(a => a.Name).ToList(),
                    ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                    UniversityName = university?.Name ?? string.Empty,
                    AccommodationType = type?.Name ?? string.Empty,
                    ApplicationStatus = status
                });
            }

            return result;
        }

        public async Task<IEnumerable<AccommodationBookingDto>> GetBookingsByStudentUserIdAsync(string studentUserId)
        {
            _logger.LogInformation("Fetching bookings for student user ID: {UserId}", studentUserId);

            var student = await _studentRepo.GetByUserIdAsync(studentUserId);
            var bookings = await _accommodationRepo.GetWithBookingsByStudentIdAsync(student.StudentId);
            var result = new List<AccommodationBookingDto>();

            foreach (var (accommodation, booking, statusName) in bookings)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(accommodation.UniversityId);
                var type = await _typeRepo.GetByIdAsync(accommodation.AccommodationTypeId);

                result.Add(new AccommodationBookingDto
                {
                    BookingId = booking.BookingId,
                    AccommodationId = accommodation.AccommodationId,
                    Title = accommodation.Title,
                    Description = accommodation.Description,
                    Address = accommodation.Address,
                    PostCode = accommodation.PostCode,
                    City = accommodation.City,
                    Country = accommodation.Country,
                    MonthlyRent = accommodation.MonthlyRent,
                    IsAvailable = accommodation.IsAvailable,
                    MaxOccupants = accommodation.MaxOccupants,
                    Size = (int)accommodation.Size,
                    AvailableFrom = accommodation.AvailableFrom,
                    AmenityNames = amenities.Select(a => a.Name).ToList(),
                    ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                    UniversityName = university?.Name ?? string.Empty,
                    AccommodationType = type?.Name ?? string.Empty,
                    BookingStatus = statusName ?? "Unknown"
                });
            }

            return result;
        }
    }
}
