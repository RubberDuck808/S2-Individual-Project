using AutoMapper;
using BLL.DTOs.Shared;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Models;
using DAL.Interfaces;

namespace BLL.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityRepository _universityRepo;
        private readonly IMapper _mapper;
        
        public UniversityService(IUniversityRepository universityRepo, IMapper mapper)
        {
            _universityRepo = universityRepo;
            _mapper = mapper;
        }


        public async Task<List<UniversityDto>> GetAllAsync()
        {
            var universities = await _universityRepo.GetAllAsync();
            return _mapper.Map<List<UniversityDto>>(universities);
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
