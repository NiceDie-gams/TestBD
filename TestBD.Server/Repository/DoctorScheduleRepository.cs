using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class DoctorScheduleRepository
    {
        private DbContext DbContext { get; }
        private DbSet<DoctorSchedule> schedule { get; }
        private readonly IMapper _mapper;

        public DoctorScheduleRepository(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext; 
            _mapper = mapper;
            schedule = DbContext.Set<DoctorSchedule>();
        }

        public async Task<ICollection<DoctorScheduleDto>> GetByEmployeeIdAndDate(Guid employeeId, DateOnly date)
        {
            var doctorSchedule = await schedule.ProjectTo<DoctorScheduleDto>(_mapper.ConfigurationProvider)
                .Where(x => x.EmployeeId == employeeId && x.PointDate == date)
                .ToListAsync();
            return doctorSchedule;
        }

        public async Task<List<DoctorScheduleWithAppointmentDto>> GetScheduleWithAppointmentsAsync(
            Guid doctorId,
            DateOnly date)
        {
            var scheduleWithAppointments = await schedule
                .Where(ds => ds.DoctorId == doctorId && ds.PointDate == date)
                .Select(ds => new DoctorScheduleWithAppointmentDto
                {
                    ScheduleNoteId = ds.ScheduleNoteId,
                    PointDate = ds.PointDate,
                    StartTime = ds.StartTime,
                    EndTime = ds.EndTime,
                    CabinetNumber = ds.CabinetNumber,
                    IsAvailable = ds.IsAvailable,

                    // Получаем запись на этот слот (если есть)
                    Appointment = ds.Appointments
                        .Select(a => new AppointmentSimpleDto
                        {
                            AppointmentId = a.AppointmentId,
                            Status = a.Status,
                            CreatedAt = a.CreatedAt,
                            Patient = new PatientSimpleDto
                            {
                                PatientId = a.Patient.PatientId,
                                PatientFio = a.Patient.PatientFio,
                                ContactPhone = a.Patient.ContactPhone
                            }
                        })
                        .FirstOrDefault() // Берем первую запись (должна быть только одна)
                })
                .ToListAsync();

            return scheduleWithAppointments;
        }

        public async Task CreateDoctorSchedule(CreateScheduleDto dto)
        {
           await DbContext.Database.ExecuteSqlRawAsync($"SELECT create_doctor_schedule( p_doctor_id => '{dto.doctorId}', p_start_date => '{dto.startDate:yyyy-MM-dd}', p_end_date => '{dto.endDate:yyyy-MM-dd}', p_cabinet_number => {dto.cabinetNumber})");
        }
    }
}
