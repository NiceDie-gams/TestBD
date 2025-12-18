using AutoMapper;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status ?? "booked"))
            .ForMember(dest => dest.EmployeeFio,
                opt => opt.MapFrom(src => src.Schedule.Doctor.EmployeeFio))
            .ForMember(dest => dest.Cabinet,
                opt => opt.MapFrom(src => src.Schedule.CabinetNumberNavigation.CabinetNumber.ToString()))
            .ForMember(dest => dest.AppointmentDate,
                opt => opt.MapFrom(src =>
                    src.Schedule.PointDate))
            .ForMember(dest => dest.AppointmentTime,
                opt => opt.MapFrom(src =>
                    (src.Schedule.StartTime)));

    }
}