using TestBD.Server.Dtos;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class AppointmentService(AppointmentRepository repository)
    {
        public async Task<ICollection<AppointmentDto>> GetAllByPatientId(Guid patientId)
        {
            return await repository.GetAllAppointmentByPatientId(patientId);
        }

        public async Task AddAsync(AppointmentRequestDto request)
        {
            await repository.AddAsync(request);
        }

        public async Task UpdateStatus(Guid id)
        {
            await repository.CancelAppointment(id);
        }

        public async Task<ICollection<AppointmentCompleteDto>> GetAllCompleteAppointmentsForEmployee(Guid id)
        {
            return await repository.GetCompletedAppointmentsByDoctorIdAsync(id);
        }
        public async Task<ICollection<AppointmentDto>> GetAllTodayAppointment()
        {
            return await repository.GetAllTodayAppointment();
        }
    }
}
