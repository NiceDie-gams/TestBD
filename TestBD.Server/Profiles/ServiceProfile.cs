using System.Runtime.ConstrainedExecution;
using AutoMapper;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Profiles
{
    public class ServiceProfile:Profile
    {
        public ServiceProfile() 
        {
            CreateMap<Service, ServiceDto>(MemberList.None);
        }
    }
}
