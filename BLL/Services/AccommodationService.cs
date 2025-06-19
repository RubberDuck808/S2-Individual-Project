using AutoMapper;
using BLL.DTOs.Accommodation;
using BLL.DTOs.Application;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly IAccommodationRepository _accommodationRepo;
        private readonly IApplicationService _applicationService;
        private readonly IAmenityService _amenityService;
        private readonly IBookingService _bookingService;
        private readonly ILandlordService _landlordService;
        private readonly IStudentService _studentService;
        private readonly IAccommodationTypeService _typeService;
        private readonly IMapper _mapper;
        private readonly IAccommodationImageService _imageService;
        private readonly IAccommodationAssemblerService _assembler;
        private readonly IUniversityService _universityService;
        private readonly IGeoLocationService _geoService;
        private readonly ILogger<AccommodationService> _logger;

        public AccommodationService(
            IAccommodationRepository accommodationRepo,
            ILandlordService landlordService,
            IAccommodationImageService imageService,
            IUniversityService universityService,
            IBookingService bookingService,
            IMapper mapper,
            IApplicationService applicationService,
            IAccommodationAssemblerService assembler,
            IAmenityService amenityService,
            IAccommodationTypeService typeService,
            IGeoLocationService geoService,
            ILogger<AccommodationService> logger,
            IStudentService studentService)
        {
            _accommodationRepo = accommodationRepo;
            _landlordService = landlordService;
            _bookingService = bookingService;
            _imageService = imageService;
            _studentService = studentService;
            _typeService = typeService;
            _universityService = universityService;
            _mapper = mapper;
            _applicationService = applicationService;
            _assembler = assembler;
            _amenityService = amenityService;
            _geoService = geoService;
            _logger = logger;
        }

        public async Task<AccommodationDto> GetByIdAsync(int id)
        {
            var entity = await _accommodationRepo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException(string.Format(ErrorMessages.AccommodationNotFound, id));
            return await _assembler.ToDtoAsync(entity);
        }

        public async Task UpdateAsync(AccommodationUpdateDto dto)
        {
            var existing = await _accommodationRepo.GetByIdAsync(dto.AccommodationId);
            if (existing == null)
                throw new NotFoundException(string.Format(ErrorMessages.AccommodationNotFound, dto.AccommodationId));


            _mapper.Map(dto, existing);
            await _accommodationRepo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var result = await _accommodationRepo.DeleteAsync(id);
            if (result == 0)
                throw new NotFoundException(string.Format(ErrorMessages.AccommodationNotFound, id));

        }

        public async Task<int> CreateAsync(AccommodationCreateDto dto, IEnumerable<int> amenityIds)
        {
            var fullAddress = $"{dto.Address}, {dto.PostCode}, {dto.City}, {dto.Country}";
            var coords = await _geoService.GetCoordinatesFromAddressAsync(fullAddress);

            var entity = _mapper.Map<Accommodation>(dto);
            if (coords == null)
            {
                throw new BadRequestException("We couldn't find your address on the map. Please double-check the input.");
            }

            var id = await _accommodationRepo.AddAsync(entity);
            await _amenityService.AddAsync(id, amenityIds);

            return id;
        }


        public async Task<int> UpdateWithAmenitiesAsync(AccommodationUpdateDto dto, IEnumerable<int> amenityIds)
        {
            var entity = _mapper.Map<Accommodation>(dto);
            await _accommodationRepo.UpdateAsync(entity);
            await _amenityService.UpdateAsync(dto.AccommodationId, amenityIds);
            return dto.AccommodationId;
        }

        public async Task<IEnumerable<AccommodationDto>> GetAllAsync()
        {
            var accommodations = await _accommodationRepo.GetAvailableAccommodationsAsync();
            var tasks = accommodations.Select(_assembler.ToDtoAsync);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<AccommodationDto>> GetIndexAsync()
        {
            var accommodations = await _accommodationRepo.GetFeaturedAccommodationsAsync();
            var tasks = accommodations.Select(_assembler.ToDtoAsync);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<LandlordAccommodationDto>> GetByLandlordUserIdAsync(string landlordUserId)
        {
            var landlord = await _landlordService.GetByUserIdAsync(landlordUserId);
            if (landlord == null)
                throw new NotFoundException($"No landlord found for user ID {landlordUserId}");

            var accommodations = await _accommodationRepo.GetAccommodationsByLandlordAsync(landlord.LandlordId);

            var applicationCounts = await _applicationService.GetApplicationCountsByLandlordIdAsync(landlord.LandlordId);

            var countDict = applicationCounts.ToDictionary(x => x.AccommodationId, x => x.Count);

            var results = new List<LandlordAccommodationDto>();

            foreach (var acc in accommodations)
            {
                var dto = _mapper.Map<LandlordAccommodationDto>(acc);

                dto.AmenityNames = await _amenityService.GetNamesByAccommodationIdAsync(acc.AccommodationId);
                dto.ImageUrls = await _imageService.GetUrlsByAccommodationIdAsync(acc.AccommodationId);
                dto.UniversityName = await _universityService.GetNameByIdAsync(acc.UniversityId);
                dto.AccommodationType = await _typeService.GetNameByIdAsync(acc.AccommodationTypeId);

                dto.ApplicationCount = countDict.TryGetValue(acc.AccommodationId, out var count) ? count : 0;

                results.Add(dto);
            }

            return results;
        }




        public async Task<IEnumerable<AppliedAccommodationDto>> GetApplicationByStudentUserIdAsync(string studentUserId)
        {
            var student = await _studentService.GetByUserIdAsync(studentUserId);
            var appTuples = await _applicationService.GetApplicationsWithAccommodationIdsByStudentAsync(student.StudentId);
            var result = new List<AppliedAccommodationDto>();

            foreach (var (applicationId, accommodationId) in appTuples)
            {
                var accommodation = await _accommodationRepo.GetByIdAsync(accommodationId);
                if (accommodation == null) continue;

                var dto = await _assembler.ToDtoAsync(accommodation);
                var status = await _applicationService.GetStatusNameByStudentAndAccommodationIdAsync(student.StudentId, accommodationId);

                result.Add(new AppliedAccommodationDto
                {
                    ApplicationId = applicationId, 
                    AccommodationId = dto.AccommodationId,
                    Title = dto.Title,
                    Address = dto.Address,
                    PostCode = dto.PostCode,
                    City = dto.City,
                    Country = dto.Country,
                    Description = dto.Description,
                    MonthlyRent = dto.MonthlyRent,
                    MaxOccupants = dto.MaxOccupants,
                    Size = dto.Size,
                    AmenityNames = dto.AmenityNames,
                    ImageUrls = dto.ImageUrls,
                    AvailableFrom = dto.AvailableFrom,
                    AccommodationType = dto.AccommodationType,
                    UniversityName = dto.UniversityName,
                    IsAvailable = dto.IsAvailable,
                    ApplicationStatus = status
                });
            }

            return result;
        }

        public async Task<IEnumerable<AccommodationBookingDto>> GetBookingsByStudentUserIdAsync(string studentUserId)
        {
            var student = await _studentService.GetByUserIdAsync(studentUserId);
            if (student == null)
                return Enumerable.Empty<AccommodationBookingDto>();

            var bookings = await _bookingService.GetByStudentAsync(student.StudentId);
            var results = new List<AccommodationBookingDto>();

            foreach (var booking in bookings)
            {
                var accommodation = await _accommodationRepo.GetByIdAsync(booking.AccommodationId);
                if (accommodation == null) continue;

                var dto = await _assembler.ToDtoAsync(accommodation); 
                var statusName = await _bookingService.GetStatusNameAsync(booking.StatusId);

                results.Add(new AccommodationBookingDto
                {
                    BookingId = booking.BookingId,
                    AccommodationId = dto.AccommodationId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Address = dto.Address,
                    PostCode = dto.PostCode,
                    City = dto.City,
                    Country = dto.Country,
                    MonthlyRent = dto.MonthlyRent,
                    IsAvailable = dto.IsAvailable,
                    MaxOccupants = dto.MaxOccupants,
                    Size = dto.Size,
                    AvailableFrom = dto.AvailableFrom,
                    AccommodationType = dto.AccommodationType,
                    UniversityName = dto.UniversityName,
                    AmenityNames = dto.AmenityNames,
                    ImageUrls = dto.ImageUrls,
                    BookingStatus = statusName
                });
            }

            return results;
        }

        public async Task<Accommodation> GetEntityAsync(int accommodationId)
        {
            var accommodation = await _accommodationRepo.GetByIdAsync(accommodationId);
            if (accommodation == null)
            {
                _logger.LogWarning("Accommodation with ID {AccommodationId} not found", accommodationId);
                throw new NotFoundException(string.Format(ErrorMessages.AccommodationNotFound, accommodationId));

            }

            return accommodation;
        }


        public async Task<IEnumerable<AccommodationBookingDto>> GetAcceptedBookingsByLandlordUserIdAsync(string landlordUserId)
        {
            var landlord = await _landlordService.GetByUserIdAsync(landlordUserId);
            if (landlord == null)
                return Enumerable.Empty<AccommodationBookingDto>();

            var accommodations = await _accommodationRepo.GetAccommodationsByLandlordAsync(landlord.LandlordId);
            var result = new List<AccommodationBookingDto>();

            foreach (var accommodation in accommodations)
            {
                var acceptedBooking = await _bookingService.GetAcceptedBookingByAccommodationIdAsync(accommodation.AccommodationId);
                if (acceptedBooking == null)
                    continue;

                var dto = await _assembler.ToDtoAsync(accommodation);

                result.Add(new AccommodationBookingDto
                {
                    BookingId = acceptedBooking.BookingId,
                    AccommodationId = dto.AccommodationId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Address = dto.Address,
                    PostCode = dto.PostCode,
                    City = dto.City,
                    Country = dto.Country,
                    MonthlyRent = dto.MonthlyRent,
                    IsAvailable = dto.IsAvailable,
                    MaxOccupants = dto.MaxOccupants,
                    Size = dto.Size,
                    AvailableFrom = dto.AvailableFrom,
                    AccommodationType = dto.AccommodationType,
                    UniversityName = dto.UniversityName,
                    AmenityNames = dto.AmenityNames,
                    ImageUrls = dto.ImageUrls,
                    BookingStatus = "Accepted"
                });
            }

            return result;
        }



    }
}