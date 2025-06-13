using AutoMapper;
using Domain.Models;
using BLL.DTOs.Student;

namespace BLL.MappingProfiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<StudentRegistrationDto, Student>();
            CreateMap<StudentUpdateDto, Student>();
        }
    }
}
