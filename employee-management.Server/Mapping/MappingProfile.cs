using AutoMapper;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Entities;

namespace employee_management.Server.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));

        // DTO to Entity mappings
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<UpdateEmployeeDto, Employee>();
    }
} 