using TestBD.Server.Dtos;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class DoctorScheduleService(DoctorScheduleRepository repository)
    {
        public async Task<ICollection<DoctorScheduleDto>> GetByEmployeeIdAndDate(Guid employeeId, DateOnly date) 
        {
            return await repository.GetByEmployeeIdAndDate(employeeId, date);
        }

        public async Task<List<DoctorScheduleWithAppointmentDto>> GetScheduleWithAppointmentsAsyn(Guid id, DateOnly date)
        {
            return await repository.GetScheduleWithAppointmentsAsync(id, date);
        }

        public async Task CreateDoctorSchedule(CreateScheduleDto dto)
        {
            await repository.CreateDoctorSchedule(dto);
        }
    }
}
