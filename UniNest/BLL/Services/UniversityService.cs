using AutoMapper;
using UniNest.BLL.DTOs.Shared;
using UniNest.BLL.Exceptions;
using UniNest.BLL.Interfaces;
using UniNest.DAL.Entities;
using UniNest.DAL.Interfaces;

namespace UniNest.BLL.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IRepository<University> _universityRepo;
        private readonly IMapper _mapper;

        public UniversityService(IRepository<University> universityRepo, IMapper mapper)
        {
            _universityRepo = universityRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UniversityDto>> GetAllAsync()
        {
            var universities = await _universityRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<UniversityDto>>(universities);
        }

        public async Task<UniversityDto> GetByIdAsync(int id)
        {
            var university = await _universityRepo.GetByIdAsync(id);
            if (university == null)
                throw new NotFoundException($"University with ID {id} not found");

            return _mapper.Map<UniversityDto>(university);
        }
    }
}
