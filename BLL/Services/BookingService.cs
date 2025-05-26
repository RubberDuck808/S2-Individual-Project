//using AutoMapper;
//using BLL.DTOs.Booking;
//using DAL.Models;
//using BLL.Exceptions;
//using DAL.Interfaces;
//using BLL.Interfaces;

//namespace BLL.Services
//{
//    public class BookingService : IBookingService
//    {
//        private readonly IBookingRepository _bookingRepo;
//        private readonly IMapper _mapper;

//        public BookingService(IBookingRepository bookingRepo, IMapper mapper)
//        {
//            _bookingRepo = bookingRepo;
//            _mapper = mapper;
//        }

//        public async Task<IEnumerable<BookingDto>> GetByStudentAsync(int studentId)
//        {
//            var bookings = await _bookingRepo.GetByStudentAsync(studentId);
//            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
//        }

//        public async Task<BookingDto> GetByIdAsync(int id)
//        {
//            var booking = await _bookingRepo.GetByIdAsync(id);
//            if (booking == null)
//                throw new NotFoundException($"Booking {id} not found");

//            return _mapper.Map<BookingDto>(booking);
//        }

//        public async Task<int> CreateAsync(BookingCreateDto dto)
//        {
//            var entity = _mapper.Map<Booking>(dto);
//            await _bookingRepo.AddAsync(entity);
//            return entity.BookingId;
//        }

//        public async Task UpdateAsync(BookingUpdateDto dto)
//        {
//            var booking = await _bookingRepo.GetByIdAsync(dto.BookingId);
//            if (booking == null)
//                throw new NotFoundException($"Booking {dto.BookingId} not found");

//            if (dto.StartDate.HasValue) booking.StartDate = dto.StartDate.Value;
//            if (dto.EndDate.HasValue) booking.EndDate = dto.EndDate.Value;
//            if (dto.StatusId.HasValue) booking.StatusId = dto.StatusId.Value;

//            await _bookingRepo.UpdateAsync(booking);
//        }

//        public async Task CancelAsync(int bookingId)
//        {
//            var booking = await _bookingRepo.GetByIdAsync(bookingId);
//            if (booking == null)
//                throw new NotFoundException($"Booking {bookingId} not found");

//            booking.StatusId = 3;
//            await _bookingRepo.UpdateAsync(booking);
//        }
//    }
//}
