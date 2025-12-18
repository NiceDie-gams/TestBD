using AutoMapper;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Profiles
{
    public class PatientProfile:Profile
    {
        public PatientProfile() 
        {
            CreateMap<PatientDto, Patient>(MemberList.None);
        }
    }
}
