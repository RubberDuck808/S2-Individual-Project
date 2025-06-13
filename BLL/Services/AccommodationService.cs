using AutoMapper;
using BLL.DTOs.Accommodation;
using DAL.Models;
using BLL.Exceptions;
using DAL.Interfaces;
using BLL.Interfaces;
using DAL.Repositories;

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



        public AccommodationService(
            IAccommodationRepository accommodationRepo,
            IMapper mapper,
            IAmenityRepository amenityRepo,
            IAccommodationImageRepository imageRepo,
            IUniversityRepository universityRepo,
            IAccommodationTypeRepository typeRepo,
            ILandlordRepository landlordRepo,
            IStudentRepository studentRepo,
            IApplicationRepository applicationRepo)
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


        public async Task<int> UpdateWithAmenitiesAsync(AccommodationUpdateDto dto, IEnumerable<int> amenityIds)
        {
            var entity = _mapper.Map<Accommodation>(dto);
            await _accommodationRepo.UpdateAsync(entity);

            await _accommodationRepo.UpdateAmenitiesAsync(dto.AccommodationId, amenityIds);
            return dto.AccommodationId;
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
                };

                dtos.Add(dto);
            }

            return dtos;
        }


        public async Task<IEnumerable<AccommodationDto>> GetIndexAsync()
        {
            var entities = await _accommodationRepo.GetFeaturedAccommodationsAsync();
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
                };

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<IEnumerable<LandlordAccommodationDto>> GetByLandlordUserIdAsync(string landlordUserId)
        {
            var landlord = await _landlordRepo.GetByUserIdAsync(landlordUserId);
            var listings = await _accommodationRepo.GetWithApplicationCountsByLandlordIdAsync(landlord.LandlordId);

            var result = new List<LandlordAccommodationDto>();

            foreach ((var accommodation, var count) in listings)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(accommodation.UniversityId);
                var type = await _typeRepo.GetByIdAsync(accommodation.AccommodationTypeId);

                var dto = new LandlordAccommodationDto
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
                };

                result.Add(dto);
            }

            return result;
        }


        public async Task<IEnumerable<AppliedAccommodationDto>> GetByStudentUserIdAsync(string studentUserId)
        {
            var student = await _studentRepo.GetByUserIdAsync(studentUserId);
            var listings = await _accommodationRepo.GetWithApplicationsByStudentIdAsync(student.StudentId);

            var result = new List<AppliedAccommodationDto>();

            foreach (var accommodation in listings)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(accommodation.UniversityId);
                var type = await _typeRepo.GetByIdAsync(accommodation.AccommodationTypeId);
                var status = await _applicationRepo.GetStatusNameByStudentAndAccommodationIdAsync(student.StudentId, accommodation.AccommodationId); // ✅

                var dto = new AppliedAccommodationDto
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
                };

                result.Add(dto);
            }

            return result;
        }



        public async Task<IEnumerable<AccommodationBookingDto>> GetBookingsByStudentUserIdAsync(string studentUserId)
        {
            var student = await _studentRepo.GetByUserIdAsync(studentUserId);
            var bookings = await _accommodationRepo.GetWithBookingsByStudentIdAsync(student.StudentId); // ✅ You'll implement this repo method

            var result = new List<AccommodationBookingDto>();

            foreach (var (accommodation, booking, statusName) in bookings)
            {
                var amenities = await _amenityRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var images = await _imageRepo.GetByAccommodationIdAsync(accommodation.AccommodationId);
                var university = await _universityRepo.GetByIdAsync(accommodation.UniversityId);
                var type = await _typeRepo.GetByIdAsync(accommodation.AccommodationTypeId);

                var dto = new AccommodationBookingDto
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
                };

                result.Add(dto);
            }

            return result;
        }

    }
}
