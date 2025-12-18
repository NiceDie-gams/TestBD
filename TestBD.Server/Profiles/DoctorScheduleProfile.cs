using AutoMapper;
using TestBD.Server.Dtos;
using TestBD.Server.Models;
namespace TestBD.Server.Profiles
{
    public class DoctorScheduleProfile : Profile
    {
        public DoctorScheduleProfile() 
        {
            CreateMap<DoctorSchedule, DoctorScheduleWithAppointmentDto>(MemberList.None);
        }
    }
}
