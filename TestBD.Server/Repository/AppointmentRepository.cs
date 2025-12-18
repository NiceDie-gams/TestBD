using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class AppointmentRepository
    {
        private DbContext DbContext { get; }
        private DbSet<Appointment> appointments { get; }
        private readonly IMapper _mapper;

        public AppointmentRepository(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            _mapper = mapper;
            appointments = DbContext.Set<Appointment>();
        }

        public async Task<ICollection<AppointmentDto>> GetAllAppointmentByPatientId(Guid patientId)
        {
            var appointment = await appointments
                .Where(a => a.PatientId == patientId && a.Status == "booked") // && a.Status == "booked"
                .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return appointment;
            
        }

        public async Task CancelAppointment(Guid id)
        {
            var service = appointments.Where(a => a.AppointmentId == id).FirstOrDefault();
            service.Status = "cancelled";
            await DbContext.SaveChangesAsync();
        }
        public async Task AddAsync(AppointmentRequestDto request)
        {
            var appointment = _mapper.Map<Appointment>(request);
            await appointments.AddAsync(appointment);
            await DbContext.SaveChangesAsync();

        }

        public async Task<ICollection<AppointmentCompleteDto>> GetCompletedAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            var query = appointments
                .Include(a => a.Patient)
                .Include(a => a.Schedule)
                .Where(a => a.Schedule.DoctorId == doctorId
                         && a.Status == "completed");

            var resAppointments = await query
                .OrderByDescending(a => a.Schedule.PointDate)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new AppointmentCompleteDto
                {
                    AppointmentId = (Guid)a.AppointmentId,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    AppointmentDate = a.Schedule.PointDate,

                    Patient = new PatientSimpleDto
                    {
                        PatientId = a.Patient.PatientId,
                        PatientFio = a.Patient.PatientFio,
                        ContactPhone = a.Patient.ContactPhone
                    },

                    ScheduleNote = new ScheduleNoteSimpleDto
                    {
                        PointDate = a.Schedule.PointDate,
                        StartTime = a.Schedule.StartTime,
                        EndTime = a.Schedule.EndTime,
                        CabinetNumber = a.Schedule.CabinetNumber
                    },

                })
                .ToListAsync();

            return resAppointments;
        }

        public async Task<ICollection<AppointmentDto>> GetAllTodayAppointment()
        {
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Now);
            var responce = await appointments
                .Where(a => a.Schedule.PointDate == todayDate)
                .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return responce;
        }
    }
}
