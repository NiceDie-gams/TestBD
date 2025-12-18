using AutoMapper;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile() 
        {
            CreateMap<Employee, EmployeeDto>(MemberList.None);
        }
    }
}
