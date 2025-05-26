//using AutoMapper;
//using BLL.DTOs.Application;
//using DAL.Models;
//using BLL.Exceptions;
//using DAL.Interfaces;
//using BLL.Interfaces;

//namespace BLL.Services
//{
//    public class ApplicationService : IApplicationService
//    {
//        private readonly IApplicationRepository _applicationRepo;
//        private readonly IMapper _mapper;

//        public ApplicationService(IApplicationRepository applicationRepo, IMapper mapper)
//        {
//            _applicationRepo = applicationRepo;
//            _mapper = mapper;
//        }

//        public async Task<IEnumerable<ApplicationDto>> GetByStudentAsync(int studentId)
//        {
//            var apps = await _applicationRepo.GetByStudentAsync(studentId);
//            return _mapper.Map<IEnumerable<ApplicationDto>>(apps);
//        }

//        public async Task<ApplicationDto> GetByIdAsync(int id)
//        {
//            var app = await _applicationRepo.GetByIdAsync(id);
//            if (app == null)
//                throw new NotFoundException($"Application {id} not found");

//            return _mapper.Map<ApplicationDto>(app);
//        }

//        public async Task<int> CreateAsync(ApplicationCreateDto dto)
//        {
//            var entity = _mapper.Map<Application>(dto);
//            await _applicationRepo.AddAsync(entity);
//            return entity.ApplicationId;
//        }

//        public async Task UpdateStatusAsync(ApplicationUpdateDto dto)
//        {
//            var app = await _applicationRepo.GetByIdAsync(dto.ApplicationId);
//            if (app == null)
//                throw new NotFoundException($"Application {dto.ApplicationId} not found");

//            app.StatusId = dto.StatusId;
//            await _applicationRepo.UpdateAsync(app);
//        }

//        public async Task<IEnumerable<ApplicationDto>> GetByLandlordAsync(int landlordId)
//        {
//            var apps = await _applicationRepo.GetByLandlordIdAsync(landlordId);
//            return _mapper.Map<IEnumerable<ApplicationDto>>(apps);
//        }
//    }
//}
